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