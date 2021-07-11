# Arlet Banking System

This project is an implementation of a banking application using C#. Users of the system include Administrators, Staffs and Customers. These users also correspond to the 3 modules supported by the system (admin, staff, customer). 

The system uses a local file for data persistence.

## Installation
### Prerequisites
This project was build using .NET Core 3.0

### Instructions
Clone the repository into your local machine. 

Copy the contents of `db.init.json` into `db.json`:
```bash
cp db.init.json db.json
```

Run this command in the root folder to start the application:
```bash
dotnet run <module_name>
```
where `module_name` is one of "admin", "staff" and "customer".

### As an admin
Run this module using:
```bash
dotnet run admin
```
The application comes shipped with a default admin with username and password as empty strings. To create a new admin, login using the default account credentials and follow the prompts to create one. You can also proceed to delete the default account.

### As a staff
Run this module using:
```bash
dotnet run staff
```
The main duty of a staff is to approve/deny customer registrations.

### As a customer
Run this module using:
```bash
dotnet run customer
```
Customers can register, change their PINs, withdraw, deposit, among other features.