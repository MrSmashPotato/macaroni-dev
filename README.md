# **Project Macaroni**

Jobilis is a gig economy and job search mobile platform application designed to seamlessly connect job seekers and employers. Whether you’re looking for freelance gigs or full-time opportunities, Jobilis makes job hunting and hiring effortless—all from the comfort of your phone. With just a few taps, users can create opportunities, discover talent, and build connections anytime, anywhere.

## **Technology Stack**

- **Frontend**: MAUI(XAML + C#)
- **Backend**: C#
- **Database**: Postgres(via Supabase Cloud) and SQLite(via C#)
- **Authentication**: Supabase Authentication

| **Internal Release Code** | **Date Released** |
| ------------------------- | ----------------- |
| MC.010.000                | 2025-02-23        |
| MC.010.001                | 2025-03-12        |
| ...                       | ...               |

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

## Important Links:

- [Design Specs](https://github.com/NykuluzC/macaroni)
