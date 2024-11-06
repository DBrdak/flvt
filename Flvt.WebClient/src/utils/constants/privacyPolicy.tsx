import React from 'react';
import { Container, Typography, Box } from '@mui/material';

const PrivacyPolicy: React.FC = () => {
    return (
        <Container maxWidth="md">
            <Box mt={4} mb={4}>
                <Typography variant="h4" gutterBottom>
                    Privacy Policy
                </Typography>

                <Typography variant="h5" gutterBottom>
                    Introduction & Purpose
                </Typography>
                <Typography paragraph>
                    This Privacy Policy explains how Flvt collects, uses, and shares information
                    when you use our web application, which enables subscribers to search for advertisements in selected locations.
                    We are committed to protecting your privacy and ensuring the security of your personal information.
                </Typography>

                <Typography variant="h5" gutterBottom>
                    Data Collected
                </Typography>
                <Typography paragraph>
                    When you register for our service, we collect the following personal data:
                </Typography>
                <ul>
                    <li>Email address, used for account creation, verification, notifications, and password recovery.</li>
                    <li>Password, which is securely stored using hashing and salting techniques.</li>
                </ul>
                <Typography paragraph>
                    Additionally, we may collect log data for troubleshooting and improving our services. Logs are stored on a private EC2 instance with Seq on AWS.
                </Typography>

                <Typography variant="h5" gutterBottom>
                    Purpose of Data Collection
                </Typography>
                <Typography paragraph>
                    We collect and process your information for the following purposes:
                </Typography>
                <ul>
                    <li>To verify and activate your account.</li>
                    <li>To send a password reset email if you request a new password.</li>
                    <li>To notify you about newly found advertisements that match your chosen filters.</li>
                </ul>

                <Typography variant="h5" gutterBottom>
                    Data Sharing
                </Typography>
                <Typography paragraph>
                    We share your data only with trusted third-party service providers to facilitate specific services:
                </Typography>
                <ul>
                    <li>Resend, for sending email notifications, password resets, and verification emails.</li>
                    <li>Amazon Web Services (AWS), which hosts our application infrastructure.</li>
                </ul>
                <Typography paragraph>
                    We do not share your information with any other third parties unless required by law.
                </Typography>

                <Typography variant="h5" gutterBottom>
                    Data Retention
                </Typography>
                <Typography paragraph>
                    We retain your personal data as long as your account remains active. If you choose to delete your account, all associated data
                    will be automatically removed from our database. Note that Resend, our email provider, may retain certain data as per their own policies.
                </Typography>

                <Typography variant="h5" gutterBottom>
                    User Rights
                </Typography>
                <Typography paragraph>
                    You have the right to:
                </Typography>
                <ul>
                    <li>Change your password directly through the web app.</li>
                    <li>Delete your account, upon which all your personal data will be removed from our database.</li>
                    <li>Request updates or changes to your email address by contacting us directly, as it cannot be changed through the app.</li>
                </ul>

                <Typography variant="h5" gutterBottom>
                    Security Measures
                </Typography>
                <Typography paragraph>
                    We implement industry-standard security practices to protect your personal information:
                </Typography>
                <ul>
                    <li>Passwords are securely stored using hashing and salting techniques.</li>
                    <li>Our application infrastructure is hosted on Amazon Web Services (AWS), utilizing AWS security features.</li>
                </ul>
                <Typography paragraph>
                    While we strive to protect your information, please note that no method of transmission or storage is completely secure.
                </Typography>

                <Typography variant="h5" gutterBottom>
                    Policy Changes & Contact Information
                </Typography>
                <Typography paragraph>
                    We may update this Privacy Policy from time to time to reflect changes in our practices. We will notify you of any
                    significant updates by email or through the web app.
                </Typography>
                <Typography paragraph>
                    If you have any questions or concerns about this Privacy Policy, please contact us at flvt@dbrdak.site.
                </Typography>
            </Box>
        </Container>
    );
};

export default PrivacyPolicy;
