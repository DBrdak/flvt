# flvt

**Extensive Apartment Rental Search App for Warsaw**

[Visit the live app](http://flvt.dbrdak.com)

flvt is a comprehensive app that aggregates apartment rental advertisements from the most popular classifieds portals in Poland. Leveraging advanced AI, flvt processes and summarizes ads to provide users with concise, concrete information—all in one place. With a clear and responsive UI, users can quickly browse through listings and find the perfect apartment. Powerful filtering options and scheduled email alerts ensure that every user can tailor the experience to their unique needs.

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
  .NET 8 with Clean Architecture principles

- **Scheduled Jobs:**  
  Multiple scheduled jobs to scrape and process rental advertisements

- **Email Service:**  
  Resend

### Infrastructure

- **Cloud Services:**  
  - AWS Lambda (serverless backend functions)  
  - AWS SQS and EventBridge (for scheduled jobs)  
  - AWS Amplify (for frontend hosting)  
  - AWS S3 (for enhanced performance and data access)  
  - API Gateway (to expose Lambda functions)

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
