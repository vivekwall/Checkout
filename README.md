**Design**

Using the same existing architetural design. But modified the controller to access a service layer to get the results, instead of accessing the repository direclty.

External Clients --> API Layer --> PaymentController --> PaymentService --> Repository --> External Bank Simulator.
![image](https://github.com/user-attachments/assets/0c22336e-735a-48cc-b132-a5c6019926e5)

**Folder Structure**

![image](https://github.com/user-attachments/assets/64847f03-88d2-4bcb-90e9-1506585220b6)

**Proposed Improvements**

* Move the sevice to different layer.
* Adding Security
* Azure Service Bus or EventGrid
* Microservice and Event based architecture
* CQRS 
* Dockerising
* Caching
* Async Operations (like PLINQ)
* Data Encryption
  
**Results**
![image](https://github.com/user-attachments/assets/88d3a9b7-c2d9-4141-8365-78783817dc75)

![image](https://github.com/user-attachments/assets/60f02ca6-9982-488e-b7cd-4bdb2dcb8d7c)

