# .NET RESTful API 組成詳解

## 目錄
- [核心元件](#核心元件)
  - [1. 專案入口點 (Program.cs)](#1-專案入口點-programcs)
  - [2. 控制器 (Controllers)](#2-控制器-controllers)
  - [3. 模型 (Models)](#3-模型-models)
  - [4. 路由 (Routing)](#4-路由-routing)
  - [5. 中介軟體 (Middleware)](#5-中介軟體-middleware)
  - [6. 服務 (Services) / 商業邏輯層](#6-服務-services--商業邏輯層)
  - [7. 依賴注入 (Dependency Injection, DI)](#7-依賴注入-dependency-injection-di)
  - [8. 設定 (Configuration)](#8-設定-configuration)
- [請求流程總結](#請求流程總結)
- [對外公開 vs. 對內運作：餐廳的比喻](#對外公開-vs-對內運作餐廳的比喻)
  - [A. 對外公開 (Public-Facing Components)](#a-對外公開-public-facing-components)
  - [B. 對內運作 (Internal Components)](#b-對內運作-internal-components)
  - [總結](#總結)
- [提供給第三方的 API 文件與資訊 (Checklist)](#提供給第三方的-api-文件與資訊-checklist)
  - [1. 環境與存取資訊 (Environment & Access)](#1-環境與存取資訊-environment--access)
  - [2. 驗證與授權機制 (Authentication & Authorization)](#2-驗證與授權機制-authentication--authorization)
  - [3. API 規格文件 (The Core API Documentation)](#3-api-規格文件-the-core-api-documentation)
  - [4. 使用規範與最佳實踐 (Usage Guidelines)](#4-使用規範與最佳實踐-usage-guidelines)
  - [5. 聯絡與支援方式 (Contact & Support)](#5-聯絡與支援方式-contact--support)
- [交付物總結](#交付物總結)

---

## 核心元件
一個典型的 .NET (更精確地說是 ASP.NET Core) RESTful API 主要由以下幾個核心部分組成：

### 1. **專案入口點 (Program.cs)**
這是應用程式的起點。在 .NET 6 及更新的版本中，這個檔案會設定並啟動 Web 伺服器、註冊服務、並配置 HTTP 請求管線 (Middleware)。

### 2. **控制器 (Controllers)**
*   **作用**：接收傳入的 HTTP 請求並回傳 HTTP 回應。這是 API 的主要工作區域。
*   **特徵**：
    *   通常繼承自 `ControllerBase`。
    *   使用 `[ApiController]` 屬性來啟用標準的 API 行為。
    *   使用路由屬性 (如 `[Route("api/[controller]")]`) 來定義 URL 結構。
    *   包含稱為 "Action 方法" 的公開方法，每個方法對應一個或多個 API 端點 (Endpoint)。
    *   Action 方法使用 `[HttpGet]`, `[HttpPost]`, `[HttpPut]`, `[HttpDelete]` 等屬性來對應 HTTP 動詞。

**範例 (`ProductsController.cs`)**:
```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet("{id}")]
    public IActionResult GetProduct(int id)
    {
        // ... 查詢並回傳產品 ...
        return Ok(new { ProductId = id, Name = "範例產品" });
    }
}
```

### 3. **模型 (Models)**
*   **作用**：定義資料的結構。這可以是你要接收的請求資料格式，或是你要回傳的回應資料格式。
*   **特徵**：
    *   通常是簡單的 C# 類別 (POCO - Plain Old CLR Object)。
    *   用於模型繫結 (Model Binding)，自動將傳入的 JSON/XML 資料轉換為 .NET 物件。
    *   也常用於定義與資料庫互動的實體 (Entity)。

**範例 (`Product.cs`)**:
```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}
```

### 4. **路由 (Routing)**
*   **作用**：將傳入請求的 URL 對應到正確的控制器 Action 方法。
*   **實現方式**：主要透過在控制器或 Action 方法上加上屬性 (Attribute-based routing) 來實現，例如 `[Route(...)]`, `[HttpGet(...)]`。

### 5. **中介軟體 (Middleware)**
*   **作用**：它是一個處理請求和回應的軟體管線 (pipeline)。每個請求都會依序通過這個管線中的元件，每個元件都可以對請求進行處理，然後將其傳遞給下一個元件，或直接回傳回應。
*   **常見用途**：
    *   **驗證 (Authentication)**：確認使用者是誰。
    *   **授權 (Authorization)**：確認使用者有權限執行此操作。
    *   **記錄 (Logging)**：記錄請求資訊。
    *   **錯誤處理 (Exception Handling)**：捕捉全域的例外狀況。
    *   **CORS (跨來源資源共用)**：允許來自不同網域的前端應用程式呼叫 API。

### 6. **服務 (Services) / 商業邏輯層**
*   **作用**：雖然可以直接在控制器中撰寫所有邏輯，但最佳實踐是將核心的商業邏輯（例如：計算、資料庫操作、呼叫其他 API）抽離到獨立的 "服務" 類別中。
*   **優點**：
    *   **關注點分離 (Separation of Concerns)**：控制器只負責處理 HTTP 相關事務。
    *   **可重用性**：多個控制器可以共用同一個服務。
    *   **可測試性**：可以獨立測試商業邏輯，而不需要模擬 HTTP 請求。

### 7. **依賴注入 (Dependency Injection, DI)**
*   **作用**：ASP.NET Core 內建了強大的 DI 容器。它負責建立和管理物件的生命週期（例如上面提到的服務或資料庫上下文）。
*   **實現方式**：在 `Program.cs` 中註冊服務 (例如 `builder.Services.AddScoped<IProductService, ProductService>();`)，然後在需要它的地方（如控制器的建構函式）直接請求介面，DI 容器會自動提供實例。

### 8. **設定 (Configuration)**
*   **作用**：管理應用程式的設定值，例如資料庫連接字串、API 金鑰等。
*   **來源**：主要來自 `appsettings.json` 和 `appsettings.Development.json` 等檔案。

---

## 請求流程總結

一個 .NET RESTful API 的請求流程大致如下：

1.  HTTP 請求到達 **Kestrel** (內建的 Web 伺服器)。
2.  請求進入 **Middleware** 管線 (執行驗證、記錄等)。
3.  **路由**系統根據 URL 找到對應的 **Controller** Action。
4.  **模型繫結**將請求內容 (如 JSON) 轉換為 **Model** 物件。
5.  Controller 透過 **DI** 取得所需的 **Service** 實例。
6.  Controller 呼叫 Service 來執行商業邏輯。
7.  Service 可能會與資料庫或其他外部資源互動。
8.  執行結果傳回 Controller。
9.  Controller 將結果包裝成 `IActionResult` (如 `Ok()`, `NotFound()`, `BadRequest()`) 回傳。
10. 結果物件被序列化成 JSON，並作為 HTTP 回應傳回給客戶端。

**時序圖 (Sequence Diagram)**

```mermaid
sequenceDiagram
    participant Client as 客戶端
    participant ASP_NET_Core as ASP.NET Core 框架 <br> (Kestrel, Middleware, Routing)
    participant Controller as 控制器
    participant Service as 服務 (商業邏輯)
    participant DB as 資料庫

    Client->>+ASP_NET_Core: 1. HTTP 請求
    Note over ASP_NET_Core: 2. 中介軟體管線處理 <br> 3. 路由解析
    ASP_NET_Core->>+Controller: 4. 呼叫對應的 Action 方法 <br> (含模型繫結)
    Note over Controller, Service: 5. 透過 DI 取得 Service
    Controller->>+Service: 6. 執行商業邏輯
    Service->>+DB: 7. 與資料庫互動
    DB-->>-Service: 回傳資料
    Service-->>-Controller: 8. 回傳執行結果
    Controller-->>-ASP_NET_Core: 9. 回傳 IActionResult (例如 Ok(data))
    ASP_NET_Core->>-Client: 10. 將結果序列化為 JSON 並回傳
```

---

## 對外公開 vs. 對內運作：餐廳的比喻

我們可以將一個 RESTful API 想像成一家餐廳，這有助於區分哪些部分是給客戶（對外）看的，哪些是支撐餐廳運作（對內）的。

*   **對外 (Public-Facing):** 這是客人（Client 端）能看到和互動的部分，像是**菜單 (API 合約)** 和 **服務生 (Controller)**。
*   **對內 (Internal):** 這是餐廳的後場，客人看不到，但支撐著整個運作，像是**廚房 (業務邏輯)**、**廚師 (Service)** 和 **儲藏室 (資料庫)**。

---

### A. 對外公開 (Public-Facing Components)

這些是 API 的消費者（例如前端網頁、手機 App 或其他後端服務）會直接接觸到的部分。它們定義了 API 的「長相」和「如何使用」。外部世界只應該知道「有哪些網址可以用」以及「傳遞的 JSON 格式長什麼樣子」。

| 元件 (Component) | 說明 | 在專案中的位置 (典型) |
| :--- | :--- | :--- |
| **控制器 (Controllers)** | API 的入口。它接收 HTTP 請求 (GET, POST, PUT, DELETE)，並決定如何回應。Controller 應該保持輕薄，像個交通警察，只負責接收請求、呼叫內部的服務來處理，然後回傳結果。 | `Controllers/` 資料夾 |
| **路由 (Routing)** | 定義了哪個 URL 對應到哪個 Controller 的哪個方法 (Action)。例如 `GET /api/products/123` 會對應到 `ProductsController` 的 `GetById(123)` 方法。 | 通常使用屬性 (Attribute) 直接定義在 Controller 的方法上，例如 `[HttpGet("{id}")]`。 |
| **資料傳輸物件 (DTOs)** | Data Transfer Objects。這是專門為了在 API 的請求 (Request) 和回應 (Response) 中傳遞資料而設計的類別。使用 DTOs 可以避免將內部的資料庫結構 (Entity) 直接暴露給外部，增加安全性與彈性。 | `Models/` 或 `DTOs/` 資料夾 |
| **API 文件 (Swagger/OpenAPI)** | 自動產生的 API 文件，讓前端或使用者能清楚地知道有哪些 API、如何呼叫、需要什麼參數。 | 在 `Program.cs` 中設定，通常網址是 `/swagger`。 |

### B. 對內運作 (Internal Components)

這些是實現 API 功能的核心，外部使用者不需要（也不應該）知道它們的存在與細節。

| 元件 (Component) | 說明 | 在專案中的位置 (典型) |
| :--- | :--- | :--- |
| **業務邏輯層 (Business Logic Layer)** | 處理所有核心商業規則的地方。通常會建立所謂的 **服務 (Services)** 或 **管理員 (Managers)** 類別來封裝這些邏輯。例如，計算訂單總價、檢查庫存、產生報告等複雜操作都在這一層完成。Controller 會呼叫這些 Service 來執行任務。 | 通常會建立一個 `Services/` 或 `Business/` 資料夾。 |
| **資料存取層 (Data Access Layer)** | 負責與資料庫溝通。它包含了所有讀取、寫入、更新、刪除資料庫資料的程式碼。最常見的是使用 Entity Framework Core，它會包含：<br> - **DbContext:** 資料庫連線的上下文。<br> - **實體 (Entities):** 直接對應到資料庫資料表的類別。 | `Data/` 或 `DataAccess/` 資料夾。Entities 可能在 `Models/Entities/`。 |
| **組態設定 (Configuration)** | 存放應用程式的設定值，例如資料庫連線字串、第三方服務的 API Key、功能開關等。這些是絕對不能對外洩漏的敏感資訊。 | `appsettings.json` 和 `appsettings.Development.json` 檔案。 |
| **中介軟體 (Middleware)** | 在請求到達 Controller 之前或回應送出之前執行的程式碼片段。常用於處理驗證 (Authentication)、授權 (Authorization)、記錄 (Logging)、錯誤處理等橫切關注點 (Cross-cutting concerns)。 | 在 `Program.cs` 中透過 `app.Use...()` 的方式設定。 |
| **依賴注入 (Dependency Injection)** | .NET 的核心機制，用來解耦合。在 `Program.cs` 中註冊服務 (例如 `builder.Services.AddScoped<IMyService, MyService>();`)，然後在需要的地方 (如 Controller) 的建構函式中注入使用，而不用自己 `new` 一個物件。 | 在 `Program.cs` 中設定。 |

### 總結

良好的 API 設計會嚴格區分這兩者。Controller 像一個乾淨的門面，只處理進出的人流與溝通；真正的複雜工作全部交由內部的廚房（業務邏輯層與資料存取層）來完成。這樣的結構讓你的 API 更安全、更容易維護與擴充。

---

## 提供給第三方的 API 文件與資訊 (Checklist)

當需要提供 RESTful API 給甲方（第三方合作夥伴）串接時，核心目標是**清晰、完整、無歧義**，讓對方的開發者可以**獨立完成**整合工作，盡可能減少來回溝通的成本。

以下是您必須提供的資訊清單，可以把它當作一個交付 checklist：

### 1. 環境與存取資訊 (Environment & Access)

這是最基本的入口資訊。

*   **API 基礎 URL (Base URL)**
    *   **測試環境 (Staging/UAT):** `https://api.staging.yourcompany.com/`
    *   **正式環境 (Production):** `https://api.production.yourcompany.com/`
    *   **務必**提供一個功能完整的測試環境，讓甲方可以在不影響正式資料的情況下進行開發與測試。

### 2. 驗證與授權機制 (Authentication & Authorization)

這是確保 API 安全的關鍵，必須描述得非常清楚。

*   **驗證方式**: 明確告知是哪一種驗證機制。
    *   **API Key**: 金鑰要放在 HTTP Header (`X-API-Key: <your_key>`) 還是 Query String (`?api_key=<your_key>`)？
    *   **OAuth 2.0**: 是哪種流程 (Flow)？例如 `Client Credentials Grant`。需要提供 Token 的獲取端點 (`/oauth/token`)、`client_id` 和 `client_secret`。
    *   **JWT Bearer Token**: 如何獲取 Token (例如透過登入 API)，以及 Token 的有效期限是多久。
*   **如何獲取憑證**: 告知甲方需要提供什麼資訊來申請他們的 API Key 或 `client_id`/`secret`。
*   **如何使用憑證**: 提供一個明確的請求範例，展示 `Authorization` 或 `X-API-Key` 標頭的確切格式。
    ```http
    GET /api/v1/orders/123
    Host: api.staging.yourcompany.com
    Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
    ```

#### JWT Bearer Token 實作範例 (ASP.NET Core)

JWT (JSON Web Token) 是目前最流行的跨域驗證解決方案之一。它是一個緊湊且獨立的標準，用於在各方之間安全地傳輸資訊 (稱為 "claims")。以下是在 ASP.NET Core 中實作它的完整步驟。

**核心流程:**

1.  **使用者登入**: 使用者提供帳號密碼。
2.  **伺服器驗證**: 伺服器驗證帳密是否正確。
3.  **核發 Token**: 驗證成功後，伺服器產生一個包含使用者資訊 (Claims) 的 JWT，並將其回傳給客戶端。
4.  **客戶端儲存 Token**: 客戶端 (如瀏覽器) 儲存此 Token。
5.  **發送請求**: 未來所有對受保護資源的請求，客戶端都必須在 HTTP Header 的 `Authorization` 欄位中攜帶此 Token。
6.  **伺服器驗證 Token**: 伺服器收到請求後，會驗證 Token 的簽章是否有效、是否過期，如果驗證通過，則允許存取。

---

**步驟 1: 安裝 NuGet 套件**

首先，在你的專案中安裝 JWT Bearer 的官方套件。

```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

---

**步驟 2: 設定 `appsettings.json`**

在 `appsettings.json` 中加入 JWT 相關的設定。金鑰 (Key) **絕對不能**外洩，且長度應該足夠長以確保安全。

```json
{
  "Jwt": {
    "Issuer": "https://yourdomain.com",
    "Audience": "https://yourapi.yourdomain.com",
    "Key": "THIS IS A SUPER SECRET KEY THAT SHOULD BE LONG AND COMPLEX"
  }
}
```
*   `Issuer`: Token 的核發者。
*   `Audience`: Token 的接收者 (你的 API)。
*   `Key`: 用於簽署 Token 的密鑰。

---

**步驟 3: 在 `Program.cs` 中設定驗證服務**

這是最核心的步驟，我們需要告訴 ASP.NET Core 如何驗證傳入的 JWT。

```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. 從 appsettings.json 讀取 JWT 設定
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

// 2. 加入驗證服務
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // 驗證發行者
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],

        // 驗證接收者
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],

        // 驗證生命週期
        ValidateLifetime = true,

        // 驗證簽署金鑰
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// 加入授權服務 (如果需要用到 Roles)
builder.Services.AddAuthorization();

var app = builder.Build();

// 3. 將驗證和授權中介軟體加入管線
// !! 注意：UseAuthentication() 必須在 UseAuthorization() 之前 !!
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
```

---

**步驟 4: 建立一個端點來核發 Token**

我們需要一個 API 端點 (例如 `POST /api/auth/login`) 來讓使用者登入並獲取 Token。

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;

    public AuthController(IConfiguration config)
    {
        _config = config;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] UserLogin model)
    {
        // **此處應替換為真實的資料庫驗證邏輯**
        if (model.Username == "test" && model.Password == "password")
        {
            var token = GenerateJwtToken(model.Username);
            return Ok(new { token });
        }

        return Unauthorized("Invalid credentials");
    }

    private string GenerateJwtToken(string username)
    {
        // 讀取 appsettings.json 中的 JWT 設定
        var jwtSettings = _config.GetSection("Jwt");
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // 定義 Claims (Token 中包含的資訊)
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            // 你可以加入自訂的 Claim，例如角色
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim("UserId", "12345") 
        };

        // 建立 Token
        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(120), // Token 過期時間
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

// 簡單的登入模型
public class UserLogin
{
    public string Username { get; set; }
    public string Password { get; set; }
}
```

---

**步驟 5: 保護你的 API 端點**

現在，你可以使用 `[Authorize]` 屬性來保護需要驗證才能存取的 Controller 或 Action。

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    [Authorize] // <-- 加在需要保護的 Action 上
    public IActionResult GetProducts()
    {
        // 因為有 [Authorize]，只有攜帶有效 Token 的請求才能進入這裡
        // 你還可以從 User Claims 中獲取資訊
        var userId = User.FindFirst("UserId")?.Value;
        return Ok(new { Message = $"Hello, user {userId}! Here are the products." });
    }

    [HttpGet("admin")]
    [Authorize(Roles = "Admin")] // <-- 只有角色為 Admin 的使用者才能存取
    public IActionResult GetAdminData()
    {
        return Ok(new { Message = "This is admin-only data." });
    }
    
    [HttpGet("public")]
    public IActionResult GetPublicData()
    {
        // 這個端點沒有 [Authorize]，任何人都可以存取
        return Ok(new { Message = "This is public data." });
    }
}
```

---

**客戶端如何使用**

1.  **POST** `https://yourapi.yourdomain.com/api/auth/login` 並附上帳號密碼的 JSON。
2.  從回應中取得 `token`。
3.  將 `token` 儲存起來。
4.  當要請求受保護的資源 (例如 `GET /api/products`) 時，在 HTTP Header 中加入：
    ```
    Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9... (很長的 token 字串)
    ```

這樣就完成了一個標準的 JWT Bearer Token 驗證與授權流程。

### 3. API 規格文件 (The Core API Documentation)

這是最重要的部分，描述了 API 的所有功能。**最佳實踐是提供一個互動式的 Swagger (OpenAPI) 文件連結**。

如果沒有 Swagger，則文件中必須包含以下**每一個 API 端點 (Endpoint)** 的詳細資訊：

*   **功能描述**: 這個 API 是做什麼的？(例如："查詢使用者資料"、"建立一筆新訂單")。
*   **路徑與方法 (Path & Method)**: 例如 `GET /api/v1/users/{userId}`。
*   **參數說明 (Parameters)**:
    *   **路徑參數 (Path Parameters)**: 如 `{userId}`，說明其型別 (整數、字串) 和意義。
    *   **查詢參數 (Query Parameters)**: 如 `?page=1&pageSize=10`，說明每個參數的名稱、型別、是否為必填、預設值和作用。
    *   **請求主體 (Request Body)**: 對於 `POST` 或 `PUT` 請求，提供完整的 JSON 格式範例，並逐一說明每個欄位的名稱、資料型別 (`string`, `number`, `boolean`, `array`, `object`)、是否必填，以及格式限制（例如日期格式為 `YYYY-MM-DD`）。
*   **成功回應 (Success Response)**:
    *   提供成功時的 HTTP 狀態碼 (例如 `200 OK`, `201 Created`)。
    *   提供成功時的回應主體 (Response Body) JSON 範例，並說明每個欄位的意義。
*   **失敗回應 (Error Response)**:
    *   說明可能發生的錯誤情境及其對應的 HTTP 狀態碼 (例如 `400 Bad Request`, `401 Unauthorized`, `404 Not Found`, `500 Internal Server Error`)。
    *   提供一個**統一的錯誤回應格式**，這非常重要。
      ```json
      {
        "errorCode": "INVALID_PARAMETER",
        "message": "The 'email' field is not a valid email address.",
        "details": "..." 
      }
      ```
*   **程式碼範例 (Code Samples)**: 提供至少一種常見語言的呼叫範例（例如 `cURL`, `JavaScript (fetch)`, `Python (requests)`）會非常有幫助。Swagger 通常能自動產生。

### 4. 使用規範與最佳實踐 (Usage Guidelines)

這部分幫助甲方更穩定、更有效率地使用您的 API。

*   **速率限制 (Rate Limiting)**: 明確告知允許的請求頻率（例如：每分鐘 100 次請求），以及超過時會收到的 HTTP 狀態碼（通常是 `429 Too Many Requests`）。
*   **版本管理 (Versioning)**: 說明您的 API 版本策略。版本號是放在 URL (`/api/v1/`) 還是 Header 中？當 API 更新時，舊版本會維護多久？
*   **分頁 (Pagination)**: 如果有列表類型的 API，請說明如何進行分頁（例如使用 `page` 和 `pageSize` 參數）。
*   **資料格式**: 強調所有請求與回應的 `Content-Type` 都是 `application/json`。

### 5. 聯絡與支援方式 (Contact & Support)

*   **技術支援聯絡窗口**: 提供一個 email 或支援平台，讓甲方在遇到問題時可以尋求協助。
*   **變更日誌 (Changelog)**: (可選，但很專業) 提供一個頁面記錄 API 的更新與變更歷史。

---

### 交付物總結

最理想的交付物是一個「**開發者整合包 (Developer Kit)**」，其中包含：

1.  一份 **Word/PDF/Markdown** 文件，涵蓋**環境、驗證、使用規範、支援方式**。
2.  一個 **Swagger/OpenAPI 文件的線上連結**，用於查閱詳細的 API 規格與線上測試。
3.  一組專門給該甲方的**測試環境憑證** (API Key 或 client_id/secret)。
