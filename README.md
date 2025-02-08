# flvt
**Extensive Apartment Rental Search App for Warsaw**

[Visit the live app](http://flvt.dbrdak.com)

flvt is a comprehensive app that aggregates apartment rental advertisements from the most popular classifieds portals in Poland. Leveraging advanced AI, flvt processes and summarizes ads to provide users with concise, concrete information—all in one place. With a clear and responsive UI, users can quickly browse through listings and find the perfect apartment. Powerful filtering options and scheduled email alerts ensure that every user can tailor the experience to their unique needs.

15,000 lines of pure code, calculated by [CLOC](https://github.com/AlDanial/cloc).

---
## Table of Contents
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Installation and Setup](#installation-and-setup)
- [Usage](#usage)
- [Configuration](#configuration)
- [Architecture Overview](#architecture-overview)
- [Known Issues](#known-issues)
- [Contributing](#contributing)
- [License](#license)
- [Acknowledgments](#acknowledgments)

---
## Features
- **Multi-Source Aggregation:**  
  Collects apartment rental ads from Poland's top classifieds portals.
- **AI-Powered Processing:**  
  Uses advanced AI to process and summarize advertisements into concise, easy-to-read formats.
- **Customizable Filtering:**  
  Powerful filters enable users to tailor searches according to their preferences.
- **Email Notifications:**  
  Users can opt to receive email alerts at specified intervals for new advertisements.

---
## Tech Stack
### Backend
- **Language & Framework:**  
  C# + .NET 8
- **Scheduled Jobs:**  
  Multiple scheduled jobs to scrape and process rental advertisements
- **Email Service:**  
  Resend
- **Database**
  DynamoDB

### Infrastructure
- **Cloud Services:**  
  - AWS Lambda (serverless backend functions)  
  - AWS SQS and EventBridge (for scheduled jobs)  
  - AWS Amplify (for frontend hosting)  
  - AWS S3 (for enhanced performance and data access)  
  - AWS API Gateway (to expose Lambda functions)
  - AWS DynamoDB (data storage)
- **Logging & Monitoring:**  
  EC2 instance running Seq for logging and performance monitoring
- **Infrastructure Management:**  
  CloudFormation for managing the infrastructure  
  Systems Manager for environment variables management

### Frontend
- **Framework & Libraries:**  
  - React  
  - Typescript  
  - Vite  
  - Material UI (MUI)
- **Performance Enhancement:**  
  IndexedDB (for in-browser data caching)

---
## Installation and Setup
This app is not primarily designed for local use. However, if you'd like to clone the repository and run it locally, please follow these steps:

### Backend
- Use the **AWS Lambda Mock Tool for .NET** to simulate AWS Lambda functions locally.
- **Deployment Recommendation:**  
  It is highly recommended to deploy the project using the provided `template.yml` file. This will set up a test environment in AWS that closely mirrors the production infrastructure.

### Frontend
1. **Clone the Repository:**
   ```bash
   git clone https://github.com/yourusername/flvt.git
   cd flvt
   ```
2. **Run the Frontend Environment:**  
   Navigate to the frontend directory (if applicable) and start the development server:
   ```bash
   npm run dev
   ```

---
## Usage
- **Aggregated Search:**  
  Users can browse through a consolidated list of apartment rental advertisements from multiple sources.
- **AI Summaries:**  
  Each ad is processed by AI to provide a succinct summary, making it easier to evaluate the listing quickly.
- **Custom Filters & Notifications:**  
  Tailor your search with custom filters and receive email notifications for new ads at your preferred intervals.

---
## Configuration
For deployment, ensure that all necessary environment variables and configuration settings are properly set. Configuration management is primarily handled through AWS Systems Manager and CloudFormation templates. Refer to the `template.yml` file for guidance on setting up your environment in AWS.

---
## Architecture Overview
This section will include detailed architecture diagrams and technical highlights of the advanced AWS infrastructure and backend design. (Coming soon.)

---
## Known Issues
**Backend Status:** The backend is currently frozen, and scheduled jobs are disabled. As a result, only "old" advertisements are available until the scheduled jobs are re-enabled.

---
## Contributing
This is a passion project, and contributions are more than welcome! If you'd like to contribute, enhance, or modify any part of the app, feel free to clone the repository and submit pull requests. Your improvements are highly appreciated.

---
## License
This project is open for public use. (Feel free to add any additional licensing details if necessary.)

---
## Acknowledgments
- **AWS Services:** For providing robust cloud infrastructure and services that power this app.
- **Resend:** For the email delivery service that keeps users updated.
- **The Open Source Community:** For the numerous tools, libraries, and frameworks that made this project possible.
