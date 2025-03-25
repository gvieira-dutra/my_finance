# My Finance

## Overview
Hi everyone, thank you for taking a look at my repo!
My Finance is a financial management system designed to help users track and organize their personal finances. 
I built it with C# and .NET, the project provides a structured and scalable approach to handle financial data, and manages transactions, budgeting, and reporting tools.

## Features
- **User Authentication**: Secure login and registration system.
- **Transaction Management**: Record income and expenses, categorizing the transactions.
- **Reports and Analytics**: Generates financial reports for better expending and income insights.
- **API Support**: RESTful API to interact with financial data.

## Project Structure
The repository follows a modular design to separate concerns effectively:
- **MyFinance.Core**: Contains business logic and data models.
- **MyFinance.API**: Exposes API endpoints for external integration.
- **MyFinance.Web**: Provides a web-based user interface for interaction.

## Technologies Used
- **Backend**: .NET Core, Entity Framework Core, ASP.NET Web API
- **Frontend**: MudBlazor
- **Database**: SQL Server / PostgreSQL
- **Authentication**: Identity Framework 

## Installation
1. Clone the repository:
   ```sh
   git clone https://github.com/gvieira-dutra/my_finance.git
   ```
2. Navigate to the project folder:
   ```sh
   cd my_finance
   ```
3. Restore dependencies:
   ```sh
   dotnet restore
   ```
4. Apply migrations to the database:
   ```sh
   dotnet ef database update
   ```
5. Run the application:
   ```sh
   dotnet run
   ```
   
## Contribution
Contributions are welcome! To contribute:
1. Fork the repository.
2. Create a new branch for your feature.
3. Commit your changes and push the branch.
4. Open a pull request.

## License
This project is licensed under the MIT License.

## Contact
For questions or support, reach out to [Gleison Vieira Dutra](https://github.com/gvieira-dutra).

