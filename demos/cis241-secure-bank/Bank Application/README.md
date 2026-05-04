Lake Bank – Secure Online Banking Application
=============================================

ASP.NET Core MVC
SQLite
Entity Framework Core
Cookie Authentication


1. HOW TO RUN
----------------------------------------

1. Run dependency restore:
   dotnet restore

2. Apply EF Core migrations:
   dotnet ef database update

3. Start the application:
   dotnet run

4. Access the site:
   https://localhost:****

5. Default admin account (seeded):
   Username: admin
   Password: Password123!


2. HTTPS / TLS CONFIGURATION
----------------------------

2.1 Development Mode (my machine)

1. The app uses ASP.NET Core’s built-in development certificate.
2. Trusted locally using:
   dotnet dev-certs https --trust
3. No TLS settings required inside appsettings.Development.json.
4. When running locally, the site automatically uses:
   https://localhost:****

2.2 Instructor / Production Setup

1. Create or obtain your own .pfx certificate stored OUTSIDE this repository.
   Example: /path/to/certs/bankapp.pfx

2. Create your own appsettings.Production.json (not included in repo):

{
  "Kestrel": {
    "Endpoints": {
      "Https": {
        "Url": "https://localhost:5001",
        "Certificate": {
          "Path": "/path/to/certs/bankapp.pfx",
          "Password": "USE_ENV_VAR"
        }
      }
    }
  }
}

3. Set the certificate password through an environment variable:
   export BANKAPP_CERT_PASSWORD="your-password"

4. Run the app in production mode:
   dotnet run --environment Production


3. APPLICATION OVERVIEW
------------------------

1. Lake Bank is a secure online banking system built with ASP.NET Core MVC.
2. Uses EF Core + SQLite for data storage.
3. Includes user registration, login, account management, money transfers, admin tools, and security auditing.
4. Implements multiple layers of security based on best practices.


4. FEATURE SUMMARY
------------------

4.1 User Functions

1. Register an account
2. Log in securely with cookie-based sessions
3. View personal accounts
4. Request new accounts
5. Transfer money between owned accounts
6. View transaction history

4.2 Admin Functions

1. Approve account requests
2. Create accounts for users
3. Lock user accounts
4. Unlock user accounts
5. Review administrator audit logs
6. Manage all users


5. DATA MODEL (EF CORE + SQLITE)
--------------------------------

The following tables are created and managed via EF Core migrations:

1. Users
2. Accounts
3. Transactions
4. AccountRequests
5. AdminAuditEntries


6. AUTHENTICATION & AUTHORIZATION
---------------------------------

1. Cookie authentication with secure session cookies.
2. Password hashing with ASP.NET Core Identity’s PasswordHasher.
3. Claims include NameIdentifier, Name, and Role.
4. All user actions require authentication.
5. All admin actions require the "AdminOnly" policy.
6. Locked users cannot authenticate.


7. SECURITY MITIGATIONS
------------------------

7.1 Password Security
1. Passwords hashed using Identity PasswordHasher.
2. No plaintext passwords stored anywhere.

7.2 Session Security
1. Cookies set HttpOnly.
2. Cookies enforced SecurePolicy = Always.
3. Session cookie regenerated on login.

7.3 CSRF Protection
1. All modifying actions use antiforgery tokens.
2. All POST endpoints use ValidateAntiForgeryToken.

7.4 Authorization Hardening
1. Account ownership validated on every action.
2. Admin tools locked behind explicit policy.

7.5 Account Lockout Controls
1. Admin can lock or unlock any user.
2. Locked users cannot sign in.

7.6 Audit Logging
1. Every admin action is permanently recorded.
2. Includes lock/unlock, account creation, approval actions.

7.7 Input Validation
1. Password confirmation required for registration.
2. Transfers require positive amounts.
3. Dropdowns limit invalid account input.

7.8 Supply Chain Security
1. Only Microsoft-authored dependencies.
2. Version-pinned and reviewed.


8. THREAT MITIGATION ANALYSIS
------------------------------

8.1 Authentication Threats
1. Password hashing prevents credential theft.
2. Unified login error prevents username discovery.
3. Locked accounts stop brute-force attempts.

8.2 Session Threats
1. HTTPS required to prevent session theft.
2. HttpOnly blocks JavaScript access.

8.3 CSRF Threats
1. Antiforgery tokens required for every POST.

8.4 Authorization Threats
1. Strict policy prevents privilege escalation.

8.5 Injection Threats
1. EF Core parameterized queries prevent SQL injection.

8.6 Auditing Strengths
1. Admin actions logged for forensic tracking.


9. MODIFYING SEED DATA
-----------------------

1. SeedData.cs contains default admin credentials and sample data.
2. Modify values directly to customize initial state.


10. FUTURE IMPROVEMENTS
------------------------

1. Multi-factor authentication
2. Email verification
3. Password reset system
4. ACH-style delayed transactions
5. Containerized deployment


11. AUTHOR
----------

Tristan Lake
CIS 225 – Secure Programming
Southeast Technical College
