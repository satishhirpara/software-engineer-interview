## Requirements
```
Microsoft Visual Studio Community 2022 (64-bit) > Version 17.4.2
.Net Core 3.1 (LTS)
```

# CSharp Project
```
- This API supports .net core 3.1 (LTS) and created from Visual studio 2022.
- This includes Depandency injection, repository pattern, SOLID principles, EF In Memory database, automapper, swagger, Enabled global cors policy (We can customise as per need),
	Azure Application insight for Exception handling which Custom MiddleWare (Need to add application telementry key for make it work), API versioning, 
	JWT Authentication (Logic added fully now to make it work we need to have separte auth api which whould generate token and same token will be validated in this api if we uncomment some 
	code like Authorise attribute and UseAuthentication in startup file),

- There are basically 3 main layers + 1 Entity + 1 test case project for service layer so total 5 project solutions
   1) Zip.InstallmentsService.API (API controllers)
   2) Zip.InstallmentsService.Core (Bussiness layer + AutoMapper)
   3) Zip.InstallmentsService.Data (Data Access layer)
   4) Zip.InstallmentsService.Entity (Dto and request/response objects)
   5) Zip.InstallmentsService.Core.Test (This is for X-Unit test cases) 
```

## All API end points
```
- Swagger UI is added for documenting all end points of the API. That can be used for api-testing as well
```

## Testing steps or details
```
Now to test this API please find below steps
- Open "Zip.InstallmentsService.sln" or project in visual studio 2022
- Make sure Zip.InstallmentsService.API project is set as a start project
- Rebuild the project so it will restore all nuget packages for the first time.
- Once build is success then run the project from visual studio 2022 and it shuld open swagger UI page (https://localhost:44336/swagger/index.html)
- Here you will see 2 apis, 1 is to create payment plan (HttpPOST) and 1 is to get payment plan by PaymentPlanId (HttpGet)
- So you can click on 'Try it out' button and provide input and test it there on swagger UI itself.
  
  OR 
- if you wish to test it via PostMan you can test it via postman as well.
  1) HttpPost > https://localhost:44336/api/PaymentPlan
	Body > 
	{
		"userId": "504A683D-B4C3-4770-962B-4B5F3F89BB91",
		"purchaseAmount": "100.00",
		"purchaseDate":"2022-01-01",
		"noOfInstallments":"4",
		"frequencyInDays":"14"
	}

  2) HttpGet > https://localhost:44336/api/PaymentPlan/{PaymentPlanId}
  Note: PaymentPlanId is the one which is creted from point1
```

## Run Tests
```
- Open "Zip.InstallmentsService.sln" or project in visual studio 2022
- Make sure Zip.InstallmentsService.Core.Test project is set as a start project
- On the top menu of visual studion, click on Test > Test Explorer. then you can get all test cases there and you can right click and run all or any single one.
```

## Assumptions made
```
- Azure app sight is used for logging as i assumed that should work after we add application insight key.
- JWT token based authontication is added with all code but that is commented so i am assuming that it should also work once we add it.
- Presently global CORS policy is enabled but i am assuming later it can be changed as per requirement.
- API versioning is done with default as a 1.0 and to check it we need to pass "X-Version = 1.0 or 2.0" in header of api. so for 1.0 we have an Api and for 2.0 we dont have an api so it will show an error.
i am assuming we will have a new api version 2.0 in future so just created folder of V2.
- Comments are now added at most of the places to get the idea of the logic however if that does not work then Name of the method is given in such a way to get some idea. 
Also i am assuming that i will follow some sort of existing commenting pattern or apply same on this project for consistancy.
```