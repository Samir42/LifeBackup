# Consuming AWS S3 SDK in ASP .NET Core 5.0 🧙‍♂️

This is the sample API with Clean Architecture implementation which demonstrates how to use Amazon S3 SDK to interact with Amazon S3 in programmatic way.

> # Prerequisities

```
1. Create a user in Amazon Console using AMI
2. Assign created user to the role called AmazonS3FullAccess
3. Check Programmatic access option 
4. When user is created, Access Key ID and Access Key will be provided. 
```

> What to do after getting access keys?

```
1. Open CMD 
2. Type aws configure
3. Input AWS Access Key 
4. Input AWS Secret Key
5. Provide Region
```
What these commands will do is they will update credentials file and write those access and secret keys into that file.


> Last preparations 
```
1. Move to the C:\Users\your_user\.aws
2. Open [credentials] file
3. Find your access key - there should be tag on top of it called [default]
4. Change [default] tag to [lifebackup-profile]. Web API will refer to the credentials file using appsettings.json file 
   to use the proper key to access Amazon S3 instance.
```

## Now, you're able to run the app. 🎉
