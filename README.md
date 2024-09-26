**Design**

Using the same existing architetural design. But modified the controller to access a service layer to get the results, instead of accessing the repository direclty.

External Clients --> API Layer --> PaymentController --> PaymentService --> Repository --> External Bank Simulator.
![image](https://github.com/user-attachments/assets/0c22336e-735a-48cc-b132-a5c6019926e5)

**Folder Structure**

![image](https://github.com/user-attachments/assets/64847f03-88d2-4bcb-90e9-1506585220b6)

**Proposed Improvements**

* Move the sevice to different layer. 
* Adding Security
* CQRS 
* Dockerising
* Caching
* Async Operations (like PLINQ)
* Data Encryption
  
