--- tags: [api] --- 


![Ref a model](../assets/images/logo.png) 
# Contribute-Tracking-System-APIs 
### **Introduction** 
What does your API do? An SMS API. It does the following: - Create contact - Delete a contact - Send message - Get list of sent messages for a particular contact - Get list of received messages for a particular conatc - Delete a particular message 
### **Overview** 
Things that the developers should know about 
### **Authentication**
 What is the preferred way of using the API? Available or Not available. Some requests require apiKey or not 
### **Error Codes** 
What errors and status codes can a user expect? - 409 for conflicts - 404 when resource cannot be found - 501 for server side error 
### **Rate limit** 
Is there a limit to the number of requests an user can send? No limit 
>Check login -ID: 189209 - Password: 123
```json http
{
  "method": "get",
  "url": "http://localhost:1037/Account/CheckLogin?id=&pw="
}
```
>Send OTP to Email 
```json http
{
  "method": "get",
  "url": "http://localhost:1037/Account/OTP?mail="
}
```
> Change password
```json http
{
  "method": "get",
  "url": "http://localhost:1037/Account/Changepassword?passnew=&apiKey="
}
```
> Delete Employee
```json http
{
  "method": "put",
  "url": "http://localhost:1037/Account/id/DeleteEmployee",
  "query": {
    "apiKey": ""
  }
}
```
> Get list rank employee
```json http
{
  "method": "get",
  "url": "http://localhost:1037/Account/RankEmployee",
  "query": {}
}
```
> Remove mission with ID
```json http
{
  "method": "put",
  "url": "http://localhost:1037/Mission/id/ClearMission",
  "query": {
    "apiKey": ""
  }
}
```
> Get list mission complete of account
```json http
{
  "method": "get",
  "url": "http://localhost:1037/Mission/ListMissionComplete",
  "query": {
    "apiKey": ""
  }
}
```
> Get information of Mission with ID
```json http
{
  "method": "get",
  "url": "http://localhost:1037/Mission/id/Describe",
  "query": {}
}
```
> Create a Mission
```json http
{
  "method": "post",
  "url": "http://localhost:1037/Mission/Create",
  "query": {
    "apiKey": ""
  },
  "body": "{\n  \"id_mission\": 0,\n  \"name_mission\": \"string\",\n  \"Stardate\": \"string\",\n  \"point\": 0,\n  \"exprie\": 0,\n  \"describe\": \"string\",\n  \"status\": true,\n  \"count\": 0,\n  \"id_type\": 0,\n  \"id_employee\": \"string\"\n}"
}
```
> Comfirm complete mission of employee
```json http
{
  "method": "put",
  "url": "http://localhost:1037/Mission/id/CompleteMission",
  "query": {
    "apiKey": ""
  }
}
```
> Confim Mission of Admin
```json http
{
  "method": "put",
  "url": "http://localhost:1037/Mission/id/Confim",
  "query": {
    "apiKey": ""
  }
}
```
> Get list mission avaliable with keyword
```json http
{
  "method": "get",
  "url": "http://localhost:1037/Mission/Search",
  "query": {
    "apiKey": ""
  }
}
```
Get list mission avaible
```json http
{
  "method": "get",
  "url": "http://localhost:1037/Missison/Missionavaible",
  "query": {
    "apiKey": ""
  }
}
```
> Get list mission process of employee
```json http
{
  "method": "get",
  "url": "http://localhost:1037/Mission/Missionavaibleemp",
  "query": {
    "apiKey": ""
  }
}
```
> Get list all mission of system
```json http
{
  "method": "get",
  "url": "http://localhost:1037/Mission/ListMission"
}
```
> Get list type mission
```json http
{
  "method": "get",
  "url": "http://localhost:1037/Type_Mission/GetAll"
}
```

