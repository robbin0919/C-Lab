* dotnet new console   --framework net6.0 --use-program-main -o MyProject  
* dotnet new console   --framework net6.0 --use-program-main -o MyLibrary  
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