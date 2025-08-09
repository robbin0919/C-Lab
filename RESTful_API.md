# .NET RESTful API 組成詳解

## 目錄
- [.NET RESTful API 組成詳解](#net-restful-api-組成詳解)
  - [目錄](#目錄)
  - [核心元件](#核心元件)
    - [1. **專案入口點 (Program.cs)**](#1-專案入口點-programcs)
    - [2. **控制器 (Controllers)**](#2-控制器-controllers)
    - [3. **模型 (Models)**](#3-模型-models)
    - [4. **路由 (Routing)**](#4-路由-routing)
    - [5. **中介軟體 (Middleware)**](#5-中介軟體-middleware)
    - [6. **服務 (Services) / 商業邏輯層**](#6-服務-services--商業邏輯層)
    - [7. **依賴注入 (Dependency Injection, DI)**](#7-依賴注入-dependency-injection-di)
    - [8. **設定 (Configuration)**](#8-設定-configuration)
  - [請求流程總結](#請求流程總結)
  - [對外公開 vs. 對內運作：餐廳的比喻](#對外公開-vs-對內運作餐廳的比喻)
    - [A. 對外公開 (Public-Facing Components)](#a-對外公開-public-facing-components)
    - [B. 對內運作 (Internal Components)](#b-對內運作-internal-components)
    - [總結](#總結)
  - [提供給第三方的 API 文件與資訊 (Checklist)](#提供給第三方的-api-文件與資訊-checklist)
    - [1. 環境與存取資訊 (Environment \& Access)](#1-環境與存取資訊-environment--access)
    - [2. 驗證與授權機制 (Authentication \& Authorization)](#2-驗證與授權機制-authentication--authorization)
      - [JWT 結構詳解 (Header, Payload, Signature)](#jwt-結構詳解-header-payload-signature)
        - [1. Header (標頭)](#1-header-標頭)
        - [2. Payload (酬載)](#2-payload-酬載)
        - [3. Signature (簽章)](#3-signature-簽章)
        - [總結](#總結-1)
      - [JWT Bearer Token 實作範例 (ASP.NET Core)](#jwt-bearer-token-實作範例-aspnet-core)
      - [JWT 驗證詳細流程解析](#jwt-驗證詳細流程解析)
        - [1. 格式檢查 (Sanity Check)](#1-格式檢查-sanity-check)
        - [2. 簽章驗證 (Signature Validation) - **最關鍵的一步**](#2-簽章驗證-signature-validation---最關鍵的一步)
        - [3. 聲明驗證 (Claims Validation)](#3-聲明驗證-claims-validation)
        - [與 `Program.cs` 的程式碼對應](#與-programcs-的程式碼對應)
        - [總結](#總結-2)
    - [3. API 規格文件 (The Core API Documentation)](#3-api-規格文件-the-core-api-documentation)
    - [4. 使用規範與最佳實踐 (Usage Guidelines)](#4-使用規範與最佳實踐-usage-guidelines)
    - [5. 聯絡與支援方式 (Contact \& Support)](#5-聯絡與支援方式-contact--support)
    - [交付物總結](#交付物總結)
  - [JWT 伺服器端驗證流程](#jwt-伺服器端驗證流程)
    - [JWT 伺服器端驗證詳細流程](#jwt-伺服器端驗證詳細流程)
      - [步驟 1：接收 Token](#步驟-1接收-token)
      - [步驟 2：檢查 Token 結構](#步驟-2檢查-token-結構)
      - [步驟 3：解碼 Header 和 Payload](#步驟-3解碼-header-和-payload)
      - [步驟 4：驗證簽名 (Signature) - **最關鍵的一步**](#步驟-4驗證簽名-signature---最關鍵的一步)
        - [「簽名不匹配」的識別原理詳解](#簽名不匹配的識別原理詳解)
          - [「簽名不匹配」的識別流程](#簽名不匹配的識別流程)
      - [步驟 5：驗證 Payload 中的 Claims (聲明)](#步驟-5驗證-payload-中的-claims-聲明)
    - [JWT 伺服器端驗證流程 (時序圖最終簡化版)](#jwt-伺服器端驗證流程-時序圖最終簡化版)
  - [附錄：保護您的 API 安全](#附錄保護您的-api-安全)
    - [如何避免權杖被盜用 (Token Theft/Replay)](#如何避免權杖被盜用-token-theftreplay)
      - [方法一：驗證權杖的預期接收者 (Audience Validation)](#方法一驗證權杖的預期接收者-audience-validation)
      - [方法二：使用寄件人約束權杖 (Sender-Constrained Tokens)](#方法二使用寄件人約束權杖-sender-constrained-tokens)
        - [1. mTLS (Mutual TLS)](#1-mtls-mutual-tls)
        - [2. DPoP (Demonstration of Proof-of-Possession)](#2-dpop-demonstration-of-proof-of-possession)
      - [總結與建議](#總結與建議)
 

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

#### JWT 結構詳解 (Header, Payload, Signature)

一個 JWT (JSON Web Token) 的內容是由**三個部分**組成的，這三個部分之間用點 (`.`) 來分隔，形成 `xxxxx.yyyyy.zzzzz` 這樣的結構。

這三個部分依序是：

1.  **Header (標頭)**
2.  **Payload (酬載)**
3.  **Signature (簽章)**

每一部分都是經過 **Base64Url** 編碼的字串。重要的是要記住，Base64Url 是**編碼**，不是**加密**，所以前兩部分的內容是可以輕易被解碼讀取的。

---

##### 1. Header (標頭)

**目的**：描述這個 JWT 的元數據（metadata），主要是關於簽章演算法的資訊。

它是一個 JSON 物件，通常包含兩個欄位：

*   `alg` (Algorithm): **必須欄位**。指定用來產生簽章的演算法，例如 `HS256` (HMAC using SHA-256) 或 `RS256` (RSA using SHA-256)。
*   `typ` (Type): **建議欄位**。宣告這是一個 JWT，其值固定為 `JWT`。

**範例 JSON**:
```json
{
  "alg": "HS256",
  "typ": "JWT"
}
```

將這個 JSON 物件進行 Base64Url 編碼後，就形成了 JWT 的第一部分。

---

##### 2. Payload (酬載)

**目的**：存放實際需要傳遞的資料，這些資料被稱為「聲明 (Claims)」。

它也是一個 JSON 物件，包含描述實體（通常是使用者）和其他相關資訊的聲明。聲明可以分為三種類型：

*   **註冊聲明 (Registered Claims)**: 這些是 IETF 官方建議的一組預先定義好的聲明，以提供一組共通的、可互通的標準。
    *   `iss` (Issuer): Token 的發行者。
    *   `sub` (Subject): Token 的主體，通常是使用者的唯一識別碼（例如 User ID）。
    *   `aud` (Audience): Token 的接收者（你的 API）。
    *   `exp` (Expiration Time): Token 的過期時間戳，**這是確保安全性的關鍵聲明**。
    *   `nbf` (Not Before): Token 的生效時間戳。
    *   `iat` (Issued At): Token 的發行時間戳。
    *   `jti` (JWT ID): Token 的唯一識別碼，可用於防止重放攻擊。

*   **公開聲明 (Public Claims)**: 由使用 JWT 的人自行定義，但為了避免衝突，應該在 [IANA JSON Web Token Registry](https://www.iana.org/assignments/json-web-token/json-web-token.xhtml) 中註冊。

*   **私有聲明 (Private Claims)**: 這是最常見的自訂聲明，由發行者和接收者雙方共同協商定義，用來傳遞非標準化但必要的資訊。例如：
    *   `role`: 使用者角色 (e.g., "Admin", "User")
    *   `username`: 使用者名稱
    *   `userId`: 使用者 ID (如果 `sub` 不敷使用)

**範例 JSON (混合了註冊聲明和私有聲明)**:
```json
{
  "sub": "1234567890",
  "name": "John Doe",
  "role": "Admin",
  "iat": 1516239022,
  "exp": 1516242622
}
```

> ⚠️ 安全警告: Payload 僅僅是經過 Base64Url 編碼，任何拿到 Token 的人都可以輕易地將其解碼並讀取內容。因此，**絕對不要在 Payload 中存放任何敏感資訊**，例如使用者的密碼、信用卡號等。

將這個 Payload JSON 物件進行 Base64Url 編碼後，就形成了 JWT 的第二部分。

---

##### 3. Signature (簽章)

**目的**：驗證 Token 的來源是否可信，並確保訊息在傳輸過程中沒有被竄改。這是 JWT 安全性的基石。

簽章是透過以下方式產生的：

1.  取前面已經過 Base64Url 編碼的 `Header`。
2.  加上一個點 (`.`)。
3.  取前面已經過 Base64Url 編碼的 `Payload`。
4.  將這個組合起來的字串，使用 `Header` 中指定的演算法 (`alg`) 和一個存放在**伺服器端的密鑰 (Secret Key)** 進行加密計算。

**公式示意**:
```
Signature = HASH_ALGORITHM(
  base64UrlEncode(header) + "." + base64UrlEncode(payload),
  your_secret_key
)
```

這個 `your_secret_key` 是存放在伺服器端的，客戶端絕對不應該知道。當伺服器收到 JWT 時，會用**同樣的方式**重新計算一次簽章，如果計算結果與收到的簽章一致，就證明 Token 是合法且未被修改的。

---

##### 總結

將這三部分編碼後的字串用點 (`.`) 連接起來，就構成了一個完整的 JWT。您可以將範例字串貼到 [jwt.io](https://jwt.io/) 網站上，它會幫您解碼並展示這三個部分的內容，這是一個學習和偵錯 JWT 的絕佳工具。

---

#### JWT Bearer Token 實作範例 (ASP.NET Core)

JWT (JSON Web Token) 是目前最流行的跨域驗證解決方案之一。它是一個緊湊且獨立的標準，用於在各方之間安全地傳輸資訊 (稱為 "claims")。以下是在 ASP.NET Core 中實作它的完整步驟。

**核心流程:**

1.  **使用者登入**: 使用者提供帳號密碼。
2.  **伺服器驗證**: 伺服器驗證帳密是否正確。
3.  **核發 Token**: 驗證成功後，伺服器產生一個包含使用者資訊 (Claims) 的 JWT，並將其回傳給客戶端。
4.  **客戶端儲存 Token**: 客戶端 (如瀏覽器) 儲存此 Token。
5.  **發送請求**: 未來所有對受保護資源的請求，客戶端都必須在 HTTP Header 的 `Authorization` 欄位中攜帶此 Token。
6.  **伺服器驗證 Token**: 伺服器收到請求後，會驗證 Token 的簽章是否有效、是否過期，如果驗證通過，則允許存取。

**時序圖 (Sequence Diagram):**

```mermaid
sequenceDiagram
    participant Client as 客戶端 (Client)
    participant AuthEndpoint as 認證端點 (/api/auth/login)
    participant Middleware as 驗證中介軟體 (Auth Middleware)
    participant ProtectedEndpoint as 受保護的端點 (/api/products)

    Note over Client, ProtectedEndpoint: 1. 認證與取得 Token

    Client->>AuthEndpoint: POST 請求 (帳號/密碼)
    activate AuthEndpoint
    AuthEndpoint->>AuthEndpoint: 驗證使用者身份
    alt 驗證成功
        AuthEndpoint->>AuthEndpoint: 產生 JWT
        AuthEndpoint-->>Client: 回傳 JWT
    else 驗證失敗
        AuthEndpoint-->>Client: 回傳 401 Unauthorized
    end
    deactivate AuthEndpoint
    Client->>Client: 儲存 JWT

    %% -- 存取階段 --

    Note over Client, ProtectedEndpoint: 2. 攜帶 Token 存取資源

    Client->>Middleware: GET 請求 (Header: Authorization: Bearer JWT)
    activate Middleware
    Middleware->>Middleware: 驗證 JWT (簽章、過期時間)
    alt JWT 有效
        Middleware->>ProtectedEndpoint: 請求合法，轉發至目標端點
        activate ProtectedEndpoint
        ProtectedEndpoint-->>Middleware: 回傳資源
        deactivate ProtectedEndpoint
        Middleware-->>Client: 200 OK (回傳資源)
    else JWT 無效
        Middleware-->>Client: 401 Unauthorized
    end
    deactivate Middleware
```

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

#### JWT 驗證詳細流程解析

當您設定好 `AddJwtBearer` 中介軟體後，每一次帶有 `[Authorize]` 屬性的 API 請求進來時，該中介軟體都會自動執行一連串的驗證步驟。以下是詳細的驗證流程：

##### 1. 格式檢查 (Sanity Check)
*   **是否存在 Token？**: 請求的 Header 中是否有 `Authorization` 欄位，且其值是否以 `Bearer ` 開頭？
*   **Token 結構是否正確？**: Token 本身是否為 `xxxxx.yyyyy.zzzzz` 的三段式結構（Header.Payload.Signature），且每段都是合法的 Base64Url 編碼？

如果這些基本檢查失敗，請求會立即被拒絕。

##### 2. 簽章驗證 (Signature Validation) - **最關鍵的一步**
這是確保 Token **未被竄改**且**來源可信**的核心步驟。

*   **解析 Token**: 中介軟體解析出 Token 的 `Header` 和 `Payload`。
*   **取得演算法與密鑰**:
    *   從 `Header` 中得知簽署時使用的演算法（例如 `HS256`）。
    *   從伺服器自身的設定檔 (`appsettings.json`) 中讀取那個**絕對不能外洩的密鑰 (Secret Key)**。
*   **重新計算簽章**: 使用相同的演算法和伺服器端的密鑰，對 `Header` 和 `Payload` **重新計算一次簽章**。
*   **比對簽章**: 將**重新計算出的簽章**與 Token 中第三部分的**原始簽章 (`Signature`)** 進行比對。

**結果**:
*   **如果兩者完全一致**: 證明這個 Token 確實是由持有正確密鑰的伺服器所簽發的，並且從簽發到現在，其內容（Header 和 Payload）完全沒有被修改過。
*   **如果兩者不一致**: 表示 Token 要嘛是偽造的，要嘛是被竄改過。請求會立即被拒絕，並回傳 `401 Unauthorized`。

##### 3. 聲明驗證 (Claims Validation)
即使簽章正確，中介軟體還需要檢查 Payload 中的「聲明 (Claims)」是否符合伺服器設定的規則。這確保了 Token 在正確的時間和場景下被使用。

*   **a. 過期時間 (`exp`)**:
    *   **檢查 `ValidateLifetime = true`**: 這個設定是否開啟？
    *   **驗證 `exp`**: 取得 Payload 中的 `exp` (Expiration Time) 聲明，確認**目前時間是否早於過期時間**。如果 Token 已過期，驗證失敗。

*   **b. 發行者 (`iss`)**:
    *   **檢查 `ValidateIssuer = true`**: 這個設定是否開啟？
    *   **驗證 `iss`**: 取得 Payload 中的 `iss` (Issuer) 聲明，確認其值是否與伺服器設定的 `ValidIssuer` (例如 `"https://yourdomain.com"`) 完全相符。這能確保 Token 是由預期的伺服器發行的。

*   **c. 接收者 (`aud`)**:
    *   **檢查 `ValidateAudience = true`**: 這個設定是否開啟？
    *   **驗證 `aud`**: 取得 Payload 中的 `aud` (Audience) 聲明，確認其值是否與伺服器設定的 `ValidAudience` (例如 `"https://yourapi.yourdomain.com"`) 完全相符。這能確保這個 Token 是要給「這個 API」使用的，而不是給其他 API 用的，防止 Token 被誤用。

##### 與 `Program.cs` 的程式碼對應
這些驗證規則都是在 `Program.cs` 中設定的：
```csharp
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // 對應步驟 3b: 驗證發行者
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],

        // 對應步驟 3c: 驗證接收者
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],

        // 對應步驟 3a: 驗證生命週期 (過期時間)
        ValidateLifetime = true,

        // 對應步驟 2: 驗證簽署金鑰
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key) // 這裡提供了驗證簽章時所需的密鑰
    };
});
```

##### 總結
整個驗證流程就像是機場的海關查驗：
1.  **格式檢查**: 護照（Token）看起來是真的嗎？格式對不對？
2.  **簽章驗證**: 護照上的防偽標籤和鋼印（Signature）是真的嗎？還是偽造的？
3.  **聲明驗證**:
    *   護照是否過期了？(`exp`)
    *   這本護照是由哪個國家發的？(`iss`)
    *   你是不是要用這本護照進入我國？(`aud`)

只有當**所有檢查都通過**，中介軟體才會認為這次請求是合法的，並將 Token 中的 Claims 解析出來賦值給 `HttpContext.User`。任何一個環節失敗，流程就會被中斷，並回傳 `4.0.1 Unauthorized`。

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


---
 

## JWT 伺服器端驗證流程

用一個簡單的譬喻來開頭：

**譬喻：主題樂園的 VIP 通行證**

1.  **發行 (Login):** 你在樂園門口（**認證伺服器**）用你的身份（帳號密碼）買了一張 VIP 通行證（**JWT**）。售票員（伺服器）在通行證上蓋了一個特殊的、無法偽造的防偽印章（**Signature**），並寫上了你的名字、VIP 等級和有效期限（**Payload/Claims**）。
2.  **驗證 (API Request):** 當你想玩任何遊樂設施（存取受保護的 **API**）時，設施管理員（**資源伺服器**）會檢查你的通行證。他不會重新去門口問你的身份，而是直接檢查：
    *   通行證是不是看起來是真的？（**格式檢查**）
    *   上面的防偽印章是不是真的？（**簽名驗證**）
    *   通行證過期了嗎？（**Claims 驗證**）

如果都沒問題，你就被允許進入設施。

---

### JWT 伺服器端驗證詳細流程

當用戶端（例如瀏覽器或手機 App）帶著 JWT 來請求一個需要保護的 API 時，伺服器會執行以下步驟：

#### 步驟 1：接收 Token

伺服器首先會從請求 (Request) 中取得 JWT。最常見的做法是從 HTTP 的 `Authorization` 標頭 (Header) 中取得。這個標頭的值通常會長這樣：

```
Authorization: Bearer <your_jwt_token>
```

伺服器會解析這個標頭，取出 `<your_jwt_token>` 的部分。

#### 步驟 2：檢查 Token 結構

一個標準的 JWT 是一個由 `.` 分隔的長字串，包含三個部分：

`Header.Payload.Signature`

伺服器會先做最基本的檢查，確認這個 Token 是否符合 `x.y.z` 的格式。如果不符合，這顯然是一個無效的 Token，直接拒絕請求。

#### 步驟 3：解碼 Header 和 Payload

JWT 的前兩部分（Header 和 Payload）是使用 Base64Url 編碼的，**並不是加密**。任何人都可以解碼它們。

*   **Header (標頭):** 通常包含演算法（如 `HS256`, `RS256`）和 Token 類型（`JWT`）。
*   **Payload (負載):** 包含一系列的 "Claims" (聲明)，這是關於用戶和 Token 本身的資訊。例如：
    *   `sub`: Subject (主題)，通常是使用者的唯一 ID。
    *   `name`: 使用者名稱。
    *   `exp`: Expiration Time (過期時間)，Token 在此時間後失效。
    *   `iss`: Issuer (發行者)，是誰發行的這個 Token。
    *   `aud`: Audience (受眾)，這個 Token 是給誰用的。

伺服器會解碼這兩部分以讀取其中的內容，準備進行下一步的驗證。

#### 步驟 4：驗證簽名 (Signature) - **最關鍵的一步**

這是整個流程中最核心的安全環節，用來確保 Token 在傳輸過程中**沒有被篡改**，並且它確實是由**合法的伺服器**所簽發的。

驗證過程取決於當初簽發時使用的演算法：

*   **對稱加密 (如 HS256):**
    1.  伺服器使用**儲存在自己內部、絕不外洩**的密鑰 (Secret Key)。
    2.  將收到的 `Header` 和 `Payload` (Base64Url 編碼後的字串) 用 `.` 連接起來。
    3.  使用與簽發時相同的演算法 (例如 HMAC-SHA256) 和同一個密鑰，對這個連接起來的字串進行加密運算，產生一個**新的簽名**。
    4.  比對這個**新產生的簽名**與收到的 JWT 中**第三部分的簽名**是否完全一致。

*   **非對稱加密 (如 RS256):**
    1.  伺服器使用**公開的金鑰 (Public Key)**。私鑰 (Private Key) 則被安全地存放在簽發 Token 的認證伺服器上。
    2.  驗證過程類似，但它會用 Public Key 來解開收到的簽名，確認它是否能與 Header 和 Payload 的雜湊值匹配。

**如果簽名不匹配，意味著：**
*   Payload 的內容被惡意修改了（例如，駭客想把自己的使用者 ID 改成管理員的 ID）。
*   這個 Token 是由一個沒有正確密鑰的偽造者所簽發的。

在這種情況下，伺服器會**立即拒絕**該請求，通常回傳 `401 Unauthorized` 或 `403 Forbidden` 的 HTTP 狀態碼。

##### 「簽名不匹配」的識別原理詳解

我們可以把這個過程想像成**銀行驗證一張支票**。

*   **支票上的簽名 (Received Signature):** 你拿著一張由 A 公司開給你的支票去銀行兌現。支票上有 A 公司財務長的親筆簽名。這個簽名就是從客戶端傳來的 JWT 中的第三部分——`Signature`。
*   **銀行存底的印鑑 (Server's Secret Key):** 銀行（**伺服器**）的資料庫裡，儲存了 A 公司開戶時留下的標準印鑑樣本。這個樣本就是伺服器內部儲存、絕不外洩的 `Secret Key`。
*   **驗證過程 (The Comparison):** 銀行櫃員（**伺服器**）不會只看支票上的簽名像不像，他會拿出存底的標準印鑑，跟支票上的金額、收款人等資訊，**重新蓋一次**在白紙上，然後比對這個**新蓋的印章**跟支票上**原有的簽名**是否**一模一樣**。

---

###### 「簽名不匹配」的識別流程

伺服器識別簽名不匹配的過程，可以分為以下三個步驟：

**步驟 1：拆解 Token**

伺服器收到一個 JWT，它看起來像 `Header.Payload.Signature`。伺服器會把它拆成三個部分：
1.  `Header` (已編碼)
2.  `Payload` (已編碼)
3.  `Received_Signature` (接收到的簽名)

`Received_Signature` 是當初發行 Token 時，用 `Secret Key` 加密前兩部分所產生的。

**步驟 2：在本機端「重新計算」一個簽名**

這是最關鍵的一步。伺服器會**完全忽略**接收到的 `Received_Signature`，並假裝自己是發行者，**重新做一次簽名流程**：

1.  拿出剛才收到的 `Header` (已編碼)。
2.  拿出剛才收到的 `Payload` (已編碼)。
3.  用一個 `.` 把這兩者連接起來，變成一個長字串：`Header + "." + Payload`。
4.  從自己的安全設定中，取出那個**絕不外洩的 `Secret Key`**。
5.  使用 `Header` 中指定的演算法（例如 HS256），搭配這個 `Secret Key`，對第 3 步的長字串進行加密運算。
6.  運算後，伺服器得到了一個**全新的、在本地端生成的簽名**。我們稱之為 `Calculated_Signature`。

**步驟 3：比對「接收的簽名」與「重新計算的簽名」**

現在伺服器手上有兩個簽名：
*   `Received_Signature`: 從 Token 中拆解出來的，由客戶端帶來的簽名。
*   `Calculated_Signature`: 伺服器剛剛用自己的密鑰重新計算出來的簽名。

伺服器會對這兩個簽名進行**逐位元 (byte-by-byte) 的精確比對**。

*   **如果完全匹配 (Match):**
    這從密碼學上證明了兩件事：
    1.  **真實性 (Authenticity):** 這個 Token 確實是由持有正確 `Secret Key` 的一方（也就是合法的伺服器）所簽發的。
    2.  **完整性 (Integrity):** `Header` 和 `Payload` 的內容從簽發那一刻到現在，**連一個字元都沒有被修改過**。因為只要內容有任何一絲變動，重新計算出來的 `Calculated_Signature` 就會與原始的完全不同。

*   **如果不匹配 (Mismatch):**
    這是一個**絕對的危險信號**，100% 代表發生了以下兩種情況之一：
    1.  **內容被竄改 (Tampered):** 有人在傳輸過程中攔截了 Token，並修改了 `Payload` 的內容。例如，一個普通用戶想把自己的 `role` 從 `"user"` 改成 `"admin"`，或者想延長 `exp` 過期時間。他雖然改了 Payload，但他沒有 `Secret Key`，所以無法產生一個能通過驗證的新簽名。
    2.  **來源是偽造的 (Forged):** 整個 Token 都是由一個惡意攻擊者產生的。他可以隨意編寫 `Header` 和 `Payload`，但他同樣沒有 `Secret Key`，所以他偽造的簽名，永遠無法和伺服器用「真密鑰」計算出來的簽名相匹配。

**結論：**
只要出現「簽名不匹配」的情況，伺服器就會**立即、無條件地**拒絕這次請求，通常會回傳 `401 Unauthorized` 錯誤，從而保護了後端的 API 資源不會被非法存取。這個比對過程是 JWT 安全機制的基石。

#### 步驟 5：驗證 Payload 中的 Claims (聲明)

即使簽名驗證通過了，伺服器還需要檢查 Payload 中的標準 Claims，確保 Token 的使用是合規的。

*   **`exp` (Expiration Time):** 檢查 Token 是否已經過期。如果當前時間晚于 `exp` 時間，則 Token 失效。
*   **`nbf` (Not Before):** 檢查 Token 是否已經生效。如果當前時間早于 `nbf` 時間，則 Token 尚未生效。
*   **`iss` (Issuer):** 檢查 Token 的發行者是否是預期的發行者。
*   **`aud` (Audience):** 檢查 Token 的接收者是否是當前的伺服器或服務。

如果任何一項檢查失敗，請求同樣會被拒絕。

### JWT 伺服器端驗證流程 (時序圖最終簡化版)

```mermaid
sequenceDiagram
    participant Client as 用戶端
    participant AuthMiddleware as 伺服器 (驗證中介軟體)
    participant ApiEndpoint as API 端點 (Controller Action)

    Client->>AuthMiddleware: GET /api/products\n(Header: Authorization: Bearer ey...)
    
    Note right of AuthMiddleware: 伺服器端內部驗證開始
    AuthMiddleware->>AuthMiddleware: 1. 取得並解析 Token
    AuthMiddleware->>AuthMiddleware: 2. 驗證簽名 (Signature)
    AuthMiddleware->>AuthMiddleware: 3. 驗證聲明 (Claims: exp, iss, aud)

    alt 驗證全部通過
        AuthMiddleware->>ApiEndpoint: 轉發已驗證的請求
        ApiEndpoint-->>AuthMiddleware: 處理完畢，回傳結果
        AuthMiddleware-->>Client: 200 OK (回傳 API 結果)
    else 任何驗證失敗
        AuthMiddleware-->>Client: 401 Unauthorized (拒絕請求)
    end
```

**時序圖解說：**

1.  **用戶端 (Client)** 向伺服器發送一個請求，並在 `Authorization` 標頭中附上 JWT。
2.  請求首先被**驗證中介軟體 (AuthMiddleware)** 截獲。
3.  中介軟體執行所有必要的**內部驗證**：解析 Token、驗證簽名、檢查是否過期等。
4.  **如果驗證成功** (`alt` 區塊的上半部):
    *   中介軟體會將請求轉發給它原本要去的 **API 端點** (例如 `ProductsController` 的某個 Action)。
    *   API 端點處理請求並回傳結果。
    *   最終伺服器向用戶端回傳 `200 OK` 及資料。
5.  **如果驗證失敗** (`else` 區塊):
    *   中介軟體會**立即中斷**流程，**不會**將請求傳遞給 API 端點。
    *   直接向用戶端回傳 `401 Unauthorized` 錯誤。

## 附錄：保護您的 API 安全

### 如何避免權杖被盜用 (Token Theft/Replay)

這是一個非常重要的安全性問題，核心在於如何防止「權杖重放攻擊」(Token Replay Attack) 或稱「權杖盜用」。單純的 Bearer Token (持有者權杖) 機制，就像一把鑰匙，任何人撿到都能開門。您需要的是一把能認得主人的鑰匙。

要解決這個問題，核心思想是：**將權杖 (Token) 與其合法的持有者 (Client) 進行唯一的綁定**。當 Server A 收到請求時，它不僅要驗證權杖本身是否有效，還要驗證發起請求的 Client 是否就是該權杖的合法主人。

以下是幾種常見且有效的解決方案，由淺入深：

#### 方法一：驗證權杖的預期接收者 (Audience Validation)

這是最基本也必須實作的一步。在產生權杖 (通常是 JWT) 時，必須明確指定這個權杖是發給「誰」用的。

*   **作法**：
    1.  在 Server A 的授權伺服器 (Authorization Server) 產生權杖時，於 JWT 的 Payload 中加入 `aud` (Audience) 聲明。這個 `aud` 的值應該是 Client A 的唯一識別碼 (例如 `client_id`)。
    2.  當 Server A 收到來自任何客戶端的請求時，除了驗證權杖的簽章、是否過期等基本檢查外，**還必須驗證權杖中的 `aud` 聲明是否與當前發起請求的客戶端 `client_id` 相符**。

*   **流程**：
    1.  Client A 攜帶自己的 `client_id` 和 `client_secret` 向 Server A 請求權杖。
    2.  Server A 驗證通過後，產生一個 JWT，其中 `aud` 聲明的值為 Client A 的 `client_id`。
    3.  Client B 盜取了這個權杖。
    4.  Client B 拿著這個權杖去呼叫 Server A 的 API。
    5.  Server A 收到請求，解析權杖後發現 `aud` 是 Client A，但發起請求的卻是 Client B (如果 Client B 無法偽造 Client A 的 `client_id` 和 `secret`)，因此拒絕該請求。

*   **優點**：實作相對簡單，是 OIDC (OpenID Connect) 的標準要求。
*   **缺點**：如果 Client B 不僅盜取了權杖，還盜取了 Client A 的 `client_id` 和 `client_secret`，那麼這個方法就會失效。它防範的是「不同客戶端」之間的盜用，但無法防範「身分被完全冒用」。

#### 方法二：使用寄件人約束權杖 (Sender-Constrained Tokens)

這是更強大的防護機制，它將權杖與客戶端持有的某個「秘密」進行密碼學上的綁定。即使權杖被盜，攻擊者沒有那個「秘密」，也無法使用。這類技術統稱為「持有證明」(Proof-of-Possession, PoP)。

##### 1. mTLS (Mutual TLS)

*   **概念**：要求客戶端在建立 TLS 連線時，也必須提供自己的客戶端憑證。權杖會與這個客戶端憑證進行綁定。
*   **作法**：
    1.  Client A 擁有一個自己的 X.509 憑證。
    2.  在向 Server A 請求權杖時，Client A 透過 mTLS 通道發送請求。
    3.  Server A 產生權杖時，會將 Client A 憑證的雜湊值 (thumbprint) 寫入 JWT 的 `cnf.x5t#S256` 聲明中。
    4.  當 Client A 帶著權杖呼叫 API 時，它必須透過同一個 mTLS 通道。
    5.  Server A 驗證收到的權杖，並確認 `cnf.x5t#S256` 的值與當前 TLS 通道中客戶端憑證的雜湊值完全相符。

*   **優點**：安全性極高，是業界公認的標準作法之一。
*   **缺點**：憑證的管理和部署對客戶端來說較為複雜，比較適合 Server-to-Server 的場景。

##### 2. DPoP (Demonstration of Proof-of-Possession)

*   **概念**：一種較新的標準 (RFC 9449)，專為無法輕易使用 mTLS 的場景設計 (例如：瀏覽器中的 SPA 應用、手機 App)。它透過客戶端簽署額外的資訊來證明自己擁有私鑰。
*   **作法**：
    1.  Client A 在本機產生一對公私鑰。
    2.  當 Client A 請求權杖時，會將「公鑰」的資訊傳給 Server A。Server A 將此公鑰的雜湊值寫入 Access Token 的 `cnf.jkt` 聲明中。
    3.  當 Client A 要呼叫 API 時，它會做兩件事：
        *   將 Access Token 放在 `Authorization` 標頭。
        *   **額外產生一個 `DPoP` 標頭**。這個標頭是一個新的、短時效的 JWT，用 Client A 的「私鑰」簽署，內容包含了 HTTP 方法 (GET/POST)、目標 URL 等資訊。
    4.  Server A 收到請求後，會進行雙重驗證：
        *   驗證 Access Token 本身。
        *   用 Access Token 中儲存的公鑰雜湊值，去驗證 `DPoP` 標頭這個 JWT 的簽章是否正確。

*   **優點**：安全性高，且比 mTLS 更靈活，適用於 Web 前端和行動應用。
*   **缺點**：客戶端和伺服器端的實作都比傳統 Bearer Token 複雜。

#### 總結與建議

| 方法 | 適用場景 | 安全等級 | 實作複雜度 |
| :--- | :--- | :--- | :--- |
| **Audience Validation** | **所有場景 (基礎必備)** | 中 | 低 |
| **mTLS** | Server-to-Server、高安全內部服務 | 高 | 高 |
| **DPoP** | Web 應用 (SPA)、行動應用 (Mobile App) | 高 | 中 |

**最佳實踐路徑**：

1.  **務必實作 Audience (`aud`) 驗證**。這是最基本的防線。
2.  根據您的客戶端類型，選擇一種持有證明 (PoP) 機制：
    *   如果您的 Client A 和 Client B 是後端服務 (Server-to-Server)，請優先考慮 **mTLS**。
    *   如果您的客戶端是瀏覽器或手機 App，請使用 **DPoP**。

透過結合「Audience 驗證」和「寄件人約束」，您就可以有效地確保從 Client A 取得的權杖，只有 Client A 能夠合法使用，從而徹底解決您的問題。
