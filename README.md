# **Project Macaroni**

A gig economy and job search mobile platform application designed to seamlessly connect job seekers and employers. Whether you’re looking for freelance gigs or full-time opportunities, Jobilis makes job hunting and hiring effortless—all from the comfort of your phone. With just a few taps, users can create opportunities, discover talent, and build connections anytime, anywhere.

## **Technology Stack**

- **Frontend**: MAUI(XAML + C#)
- **Backend**: C#
- **Database**: Postgres(via Supabase Cloud) and SQLite(via C#)
- **Authentication**: Supabase Authentication

| **Internal Release Code** | **Date Released** |
| ------------------------- | ----------------- |
| MC.010.000                | 2025-02-23        |
| MC.010.001                | 2025-03-12        |
| MC.010.002                | 2025-04-23        |
| MC.010.003                | 2025-04-27        |


## MC.010.000 Release Notes

- Initialization of Project Structure
- Initialization of Database(Supabase)
- Initialization of Project Core Dependencies and Packages

## MC.010.001 Release Notes

- Segregated Files by its function following MVVM standards(Files are categorized: Models,ViewModels,Views and Services[For APIs and external connections]).
- Implemented Application's connection to the Database.
- Pre-Implemented Database Authentication functionalities.
- Implemented Prototype Pages: Login,Registration,Verification and Home/Dashboard.
- Implemented Prototypes of Essential Authentication Components: Navigation Menu, Logout Button, Error Dialogs.

**Known Issues**:

- 1. Environment variables are still not handled meaning the database api is still exposed(though it is just a test account api its still a bad practice)
- 2. UI and UX of each pages are still not styled and designed yet.
- 3. Navigation still have issues, A bug happens if Login page is made to be the first page in the navigation stack (possible solution: https://www.youtube.com/watch?v=Jl2xUIUsZLI)
## MC.010.002 Release Notes

- Implemented the following pages:
  - Profile Page
  - Job Post Page
- Implemented Session Handling:
  - User session is now saved in the device to avoid re-login
  - User session is now saved in the database to avoid re-login
 
## MC.010.003 Release Notes

- Implemented the following page/s:
  - Registration Completion Popup Page
- Revised the following page/s:
  - Complete Registration Page --> Turned into Popup
- Feature added:
  - Upload Profile Picture now is available in Complete Registration Popup
  - Dynamic Profile Page based on individual profile refering to Supabase's values
- UI/UX Improvements:
  - Changed the overall look of the Job management Section of Profile Page
- Backend Improvements:
  - Restructed Dependency Injection and Services to use proper singleton adhering to standard practice improving scalability
  - Restructed User Authentication to rely on Supabase's auth password encryption, avoiding storing native string passwords stored in users table in the database and removing redundant data.

**Known Issues**:
  - Third Party Login is for now not working due to lack of adjustment towards the new Auth process

## Important Links:

- [Design Specs](https://github.com/NykuluzC/macaroni)
