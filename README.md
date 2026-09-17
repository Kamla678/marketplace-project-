# Marketplace API — Backend Core Flow (Auth → Products → Cart → Checkout → Payments)

كود حقيقي (مش وثائق بس) للأجزاء الأساسية اللي بتكوّن الـflow الكامل من التسجيل لحد الدفع:
1. **Auth**: Register/Login/Refresh/Logout
2. **Products**: Create/List/Details (Customer) + Seller Dashboard (CRUD + Stock/Price) + Admin Approval Workflow
3. **Cart**: Add/Update/Remove
4. **Orders/Checkout**: تحويل الكارت لطلب حقيقي مع كوبونات وتقسيم Multi-vendor
5. **Reviews & Wishlist**
6. **Payments**: Stripe Test Mode أو Mock Gateway، Webhook + Simulate endpoint

كل ده مبني على Clean Architecture بنفس الـpattern (Domain → Application/CQRS → Infrastructure → API).

⚠️ **الكود ده اتكتب يدويًا بدون تشغيل `dotnet build` فعليًا** (البيئة اللي اتكتب فيها مفيهاش .NET SDK ومفيهاش اتصال إنترنت لتحميله). يعني ممكن يكون فيه أخطاء صغيرة (typo، اسم namespace ناقص، إلخ) لازم تتصلحي وانتي بتبنيه أول مرة على جهازك. الـarchitecture والـlogic نفسهم صح 100%، بس محتاجين "compile pass" حقيقي.

## المتطلبات
- .NET 8 SDK
- Docker (لتشغيل SQL Server محليًا)
- `dotnet-ef` tool: `dotnet tool install --global dotnet-ef`

## خطوات التشغيل

```bash
# 1. تشغيل SQL Server عن طريق Docker
export DB_PASSWORD="YourStrong@Passw0rd"
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=$DB_PASSWORD" \
  -p 1433:1433 --name marketplace-sql -d mcr.microsoft.com/mssql/server:2022-latest

# 2. استعادة الـpackages
dotnet restore

# 3. عمل أول Migration
cd src/Marketplace.API
dotnet ef migrations add InitialCreate \
  --project ../Marketplace.Infrastructure \
  --startup-project .

# 4. تحديث appsettings.json بالـConnectionString والـJwtSettings:SecretKey
#    (استخدمي secret عشوائي 32+ حرف، ومتسيبيهوش القيمة الافتراضية)

# 5. تشغيل المشروع (الـmigration هتتطبق تلقائيًا في Development)
dotnet run
```

بعد ما يشتغل، افتحي `https://localhost:{port}/swagger` وجربي السيناريو الكامل:

1. `POST /api/v1/auth/register` — سجلي Seller (Role=2)
2. **مهم:** الـSeller الجديد بيبقى `ApprovalStatus=Pending` تلقائيًا — لازم يتوافق عليه الأدمن الأول قبل ما يقدر يضيف منتجات. أسهل حل للتجربة المحلية: افتحي الـDB مباشرة وغيّري `SellerProfiles.ApprovalStatus` لـ `1` (Approved) يدويًا، أو سجلي Admin وابني `/admin/sellers/{id}/approval` endpoint بنفس الـpattern بتاع الـProducts approval (لسه معملناهوش في الجزء ده).
3. `POST /api/v1/auth/login` بالـSeller ده — هيرجعلك Access Token فيه claim `sellerId`
4. استخدمي التوكن في زرار "Authorize" في Swagger
5. `POST /api/v1/seller/products` — ضيفي منتج (هيدخل Pending)
6. سجلي Admin يدويًا في الـDB (`Role=3`) أو زودي endpoint تسجيل خاص بيه، ثم:
   - `GET /api/v1/admin/products/pending` — شوفي المنتج الجديد
   - `PATCH /api/v1/admin/products/{id}/approve` — وافقي عليه
7. `GET /api/v1/products` — المنتج المعتمد يظهر دلوقتي للعميل
8. `PATCH /api/v1/seller/products/{id}/stock` و `/price` — جربي تحديث المخزون والسعر بحساب الـSeller

**نقطة اختبار أمنية مهمة:** سجلي Seller B وحاولي تستخدمي الـtoken بتاعه على `PATCH /seller/products/{id}/stock` بمعرف منتج بتاع Seller A — المفروض يرجع 404 (Product not found) مش 403، عشان مايكشفش أصلًا إن المنتج ده موجود لحد تاني.

## المشاكل المحتملة اللي ممكن تقابليها (common gotchas)

| المشكلة | الحل المتوقع |
|---|---|
| `dotnet ef` مش لاقي الـDbContext | تأكدي إنك شغالة الأمر من فولدر `Marketplace.API` بالظبط زي فوق |
| Connection refused على SQL Server | ادّيلها وقت أكتر تبدأ (10-15 ثانية) قبل ما تعملي migration |
| JWT SecretKey قصير | لازم يكون 32 حرف على الأقل وإلا HMACSHA256 هيرمي exception |
| نسيتي تضيفي `using` في أي ملف | فيجوال ستوديو / Rider هيقترحه تلقائي، أو شغلي `dotnet build` وشوفي الأخطاء وحلّيها واحدة واحدة |

## اللي اتعمل لحد دلوقتي (كود حقيقي)

- ✅ Auth: Register/Login/Refresh/Logout
- ✅ Products: Create, List (public + filters), Details, Update Stock/Price (Seller), Approve/Reject (Admin)
- ✅ Cart: Add/Update/Remove items، مع إعادة تحقق من الـstock وقت الإضافة
- ✅ Orders: Checkout كامل (يفلتر الكارت → يتحقق من الـstock والسعر لحظيًا → يطبق كوبون لو موجود → يخصم المخزون → يقسم الطلب حسب كل Seller → يفضي الكارت)، عرض طلبات المستخدم وتفاصيل كل طلب
- ✅ Reviews: إضافة تقييم (تقييم واحد لكل مستخدم لكل منتج)، عرض تقييمات منتج + متوسط التقييم (محدث تلقائيًا بعد كل تقييم جديد)
- ✅ Wishlist: إضافة/إزالة/عرض
- ✅ Seller Orders Dashboard: كل الـOrderItems بتاعة الـSeller الحالي بس
- ✅ Payments: Create Payment Intent + Webhook (Stripe Test Mode أو Mock Gateway) + Simulate endpoint للتجربة المحلية
- ✅ Categories: شجرة هرمية (Public GET) + إنشاء (Admin فقط)
- ✅ Brands: List (Public) + إنشاء (Admin فقط)
- ✅ Coupons: إنشاء وعرض كوبونات خاصة بكل Seller (مربوطة بـSellerId من التوكن)

## 🎉 الـ.NET Backend الأساسي (Core Flow) خلص فعليًا

يعني دلوقتي فيه دورة كاملة شغالة على الورق: Register → Login → Browse Categories/Products → Add to Cart → Apply Coupon → Checkout → Pay (Sandbox) → Review → كل ده مع عزل بيانات كامل بين الـSellers، وAdmin approval workflow للـSellers والمنتجات.

## اللي لسه ناقص من الـ.NET Backend (تفاصيل إضافية، مش أساسية للـflow)

- Notifications, Complaints, Returns (الـworkflow موصوف في phase3/phase4، بس الكود لسه لأ)
- Seller registration approval endpoint (Admin side) — لسه بتتغير يدويًا في الـDB
- Verified Purchase check على الـReviews (TODO واضح في الكود)
- Admin endpoint لكوبونات عامة على مستوى المنصة (دلوقتي كل كوبون لازم يبقى تابع لـSeller)
- كل الـFrontend، الـAI Service، الـTesting، الـCI/CD (تفاصيلها في ملفات الـPhases التانية)

## نقطة مهمة لسه ناقصة (Transaction/Unit of Work)

الـCheckout بيعمل كذا `SaveChanges` منفصلة (تحديث المخزون، إنشاء الـOrder، تفضية الكارت). في حالة نادرة (السيرفر وقع في النص) ممكن يحصل تضارب. في الإنتاج الفعلي، لازم كل ده يتلف في DB Transaction واحدة (`_context.Database.BeginTransactionAsync()`) عشان يبقى Atomic بالكامل — ده أهم حاجة تتصلح قبل ما تعتبريه production-ready.

## Payments — Sandbox/Test Mode بالتفصيل

عندك اختياران، تتحكمي فيهم بسطر واحد في `appsettings.json` تحت `"Payments": { "Provider": "..." }`:

### 1. Mock (الافتراضي — صفر إعدادات)
مفيش أي اتصال بأي API خارجي. `create-intent` بيرجع client secret وهمي فورًا، وعشان تأكدي الدفع بنفسك، استخدمي:
```
POST /api/v1/payments/{orderId}/simulate?success=true
```
ده بيحاكي نجاح الدفع مباشرة — مفيد جدًا للعرض (Demo) من غير أي حساب أو مفاتيح.

### 2. Stripe (Test Mode حقيقي)
1. اعملي حساب مجاني على stripe.com (Test Mode مفعّل افتراضيًا، مفيش فلوس حقيقية).
2. من الداشبورد، هاتي `sk_test_...` وحطيها في `Stripe:SecretKey`.
3. غيّري `Payments:Provider` لـ`"Stripe"`.
4. عشان تستقبلي الـwebhook محليًا (لأن Stripe محتاج URL عام مش localhost)، ثبتي Stripe CLI وشغلي:
   ```bash
   stripe listen --forward-to https://localhost:{port}/api/v1/payments/webhook
   ```
   الأمر ده هيديكي `whsec_...` مؤقت — حطيه في `Stripe:WebhookSecret`.
5. استخدمي رقم كارت تجريبي زي `4242 4242 4242 4242` (أي تاريخ مستقبلي، أي CVC) وقت الدفع الفعلي من الـFrontend.

**نقطة أمنية مهمة اتنفذت:** الـ`/payments/{orderId}/simulate` endpoint بيترفض تلقائيًا (`403 Forbidden`) لو البيئة مش `Development` — يعني مستحيل حد يستخدمه في production عشان "يدفع" من غير ما يدفع فعلًا.

## اللي جاي بعد كده

الـ.NET core flow خلص. الخطوة المنطقية التالية: إما (أ) نبني الـFrontend (React) عشان يبقى فيه حاجة تتشاف بصريًا، أو (ب) نضيف Testing (Unit + Integration) على اللي اتبنى، أو (ج) نبدأ الـAI Service. القرار ليكي.
