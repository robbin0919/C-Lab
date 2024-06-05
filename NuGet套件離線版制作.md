如果您無法通過 Internet 連線直接安裝 NuGet 套件，則可以從另一台已安裝 NuGet 套件的主機複製套件。以下是如何操作：  

**查找 NuGet 套件路徑：**在已安裝 NuGet 套件的主機上，找到 NuGet 套件的路徑。默認情況下，NuGet 套件位於 [USERPROFILE]\.nuget\packages 目錄中。  

**複製 NuGet 套件：**將所需的 NuGet 套件複製到可訪問的外部存儲設備，例如 USB 驅動器或外部硬盤驅動器。  

**將 NuGet 套件複製到目標主機：**將包含 NuGet 套件的外部存儲設備連接到無法連接到 Internet 的目標主機。  

**將 NuGet 套件複製到目標主機的 NuGet 目錄：**將複製的 NuGet 套件複製到目標主機的 NuGet 目錄（通常位於 [USERPROFILE]\.nuget\packages）。  

**還原 NuGet 緩存：**在目標主機上運行以下命令以還原 NuGet 緩存：  
```
dotnet nuget locals --clear
dotnet nuget clear cache
```
**還原項目的依賴項：**在目標主機上運行以下命令以還原項目的依賴項：  
```
dotnet restore
```
**發佈應用程式：**運行 dotnet publish 命令以發佈您的應用程式。  
