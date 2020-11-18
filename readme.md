# API localhost user guide

### Running the API 

Start AthleetAPI project in *Visual Studio*
---
Test connection by entering https://localhost:44394/api/Users in your browser and checking for JSON output

## Exporting SSL key

In Chrome:

    1. Click on the *Lock" icon next to the API url 
    2. Click on *Certificate*
    3. Navigate to *Details* tab
    4. Click on *Copy to File*
    5. Export certificate as *DER Encoded Binary X.509* (first option) and save as **cert.cer**

## Adding SSL Cert to Java Store
Navigate to \Android\Android Studio\bin

Open cmd as administrator

Enter:

`keytool -importcert -alias AthleetCert -keystore "$YOUR_PATH_TO_KEYSTORE\Android\Android Studio\jre\jre\lib\security\cacerts" -file "$YOUR_PATH_TO_CERT\cert.cer"`

default pass is **changeit**

## Adding sample connection in Android Studio 

Instantiating new client

         val client = OkHttpClient().newBuilder().build() 

Building new connection string to your api address

        val retrofit = Retrofit.Builder()

        .baseUrl("https://localhost:44394/api/")

        .client(client).addConverterFactory(GsonConverterFactory.create())

        .build()'

Create a service using the application AthleetService library and methods

         val service = retrofit.create(AthleetService::class.java)

Call object stores any service functions; can be used in unit testing

         val call = service.myFunction()