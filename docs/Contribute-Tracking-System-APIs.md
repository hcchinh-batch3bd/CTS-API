![Ref a model](../assets/images/logo.png) 
# Contribute-Tracking-System-APIs 
### **Introduction** 
What does your API do? 
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
  "url": "https://api.hotrogame.online/Account/CheckLogin?id=&pw="
}
```
>Send OTP to Email 
```json http
{
  "method": "get",
  "url": "https://api.hotrogame.online/Account/OTP?mail="
}
```
> Change password
```json http
{
  "method": "put",
  "url": "https://api.hotrogame.online/Account/Changepassword?passnew=&apiKey="
}
```
> Delete Employee
```json http
{
  "method": "put",
  "url": "https://api.hotrogame.online/Account/id/DeleteEmployee",
  "query": {
    "apiKey": ""
  }
}
```
> Get list rank employee
```json http
{
  "method": "get",
  "url": "https://api.hotrogame.online/Account/RankEmployee",
  "query": {}
}
```
> Remove mission with ID
```json http
{
  "method": "put",
  "url": "https://api.hotrogame.online/Mission/id/ClearMission",
  "query": {
    "apiKey": ""
  }
}
```
> Get list mission complete of account
```json http
{
  "method": "get",
  "url": "https://api.hotrogame.online/Mission/ListMissionComplete",
  "query": {
    "apiKey": ""
  }
}
```
> Get information of Mission with ID
```json http
{
  "method": "get",
  "url": "https://api.hotrogame.online/Mission/id/Describe",
  "query": {}
}
```
> Create a Mission
```json http
{
  "method": "post",
  "url": "https://api.hotrogame.online/Mission/Create",
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
  "url": "https://api.hotrogame.online/Mission/id/CompleteMission",
  "query": {
    "apiKey": ""
  }
}
```
> Confirm Mission of Admin
```json http
{
  "method": "put",
  "url": "https://api.hotrogame.online/Mission/id/Confim",
  "query": {
    "apiKey": ""
  }
}
```
> Get list mission available with keyword
```json http
{
  "method": "get",
  "url": "https://api.hotrogame.online/Mission/Search",
  "query": {
    "apiKey": ""
  }
}
```
Get list mission availble
```json http
{
  "method": "get",
  "url": "https://api.hotrogame.online/Missison/Missionavailable",

}
```
> Get list mission process of employee
```json http
{
  "method": "get",
  "url": "https://api.hotrogame.online/Mission/Missionavailableemp",
  "query": {
    "apiKey": ""
  }
}
```
> Get list all mission of system
```json http
{
  "method": "get",
  "url": "https://api.hotrogame.online/Mission/ListMission"
}
```
> Get list type mission
```json http
{
  "method": "get",
  "url": "https://api.hotrogame.online/Type_Mission/GetAll"
}
```


