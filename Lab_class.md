* dotnet new console   --framework net6.0 --use-program-main -o MyProject  
* dotnet new mvc   --framework net6.0   -o MyProject
* dotnet new classlib --framework net6.0  -o MyLibrary  
* dotnet add MyProject reference MyLibrary  
# 建立新專案
dotnet new classlib -o MyLibrary

# 開啟專案
code MyLibrary

# 編寫程式碼 (在 MyLibrary/Class1.cs 中)
// ... (如上所示)

# 建置專案
dotnet build

# 在另一個專案中使用 MyLibrary
dotnet add MyProject reference MyLibrary


https://www.youtube.com/watch?v=hypDgKReP0c  

https://github.com/pierre3/PlantUmlClassDiagramGenerator  

https://www.google.com/search?sca_esv=c839f9702c677c11&sxsrf=ADLYWILper0rj-uGs0H_hOmX0u3w3d5HrA:1722182726918&q=PlantUmlClassDiagramGenerator&tbm=vid&source=lnms&fbs=AEQNm0AaBOazvTRM_Uafu9eNJJzC3QMRKTS5UIeA1ZwBo3sfI5tRK2wzmp0oTr82Uvr9kDU5RO0xZEAgPH8kQ6uw4YLuM6T_apVVdTQK2BovwLOmOMtzJetbZo2aEcROTHlnQzQx_K9mKaP12swp0KKyixB3EScepO_kLnSnaLsL4A4rsRXTpGGbb5EfDM7l5v8Kxa80KEoi&sa=X&ved=2ahUKEwjykLTNjsqHAxXtbPUHHaDQIPgQ0pQJegQIBhAB&cshid=1722182805424974&biw=2560&bih=1277&dpr=1.5  
