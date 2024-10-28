using System.Reflection.PortableExecutable;

namespace Flvt.Infrastructure.Messanger.Emails.Models;

internal class TemplateBuilder
{
    private static string FromBaseVerificationCodeTemplate(string header, string message, string code) =>
        $$"""
          <!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
          <html dir="ltr" lang="en">
            <head>
              <meta charset="UTF-8">
              <title>Verification</title>
              <style type="text/css">
                body {
                  font-family: Arial, sans-serif;
                  background-color: #f4f4f4;
                  margin: 0;
                  padding: 0;
                }
          
                .container {
                  width: 100%;
                  max-width: 600px;
                  margin: 0 auto;
                  background-color: #ffffff;
                  padding: 20px;
                  border-radius: 8px;
                  box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                }
          
                .header {
                  background-color: #44017a;
                  color: #ffffff;
                  padding: 10px 20px;
                  text-align: center;
                  border-radius: 8px 8px 0 0;
                }
          
                .content {
                  padding: 20px;
                  font-size: 16px;
                  line-height: 1.6;
                }
          
                .verification-code {
                  text-align: center;
                  display: block;
                  width: fit-content;
                  margin: 20px auto;
                  padding: 10px 20px;
                  background-color: #f4f4f4;
                  border: 1px solid #dddddd;
                  border-radius: 4px;
                  font-size: 24px;
                  font-weight: bold;
                  color: #44017a;
                }
          
                .verification-code-digit {
                  display: inline;
                  margin: 1rem;
                }
          
                .footer {
                  text-align: center;
                  padding: 10px;
                  font-size: 12px;
                  color: #aaaaaa;
                }
          
                ul li,
                ol li {
                  margin-left: 0;
                }
          
                .es-p-default {
                  padding-top: 0px;
                  padding-right: 0px;
                  padding-bottom: 0px;
                  padding-left: 0px;
                }
          
                .es-p-all-default {
                  padding: 0px;
                }
          
                @media only screen and (max-width:600px) {
                  .h-auto {
                    height: auto !important
                  }
                }
          
                @media screen and (max-width:384px) {
                  .mail-message-content {
                    width: 414px !important
                  }
                }
              </style>
            </head>
            <body data-new-gr-c-s-loaded="14.1204.0">
              <div class="container">
                <div class="header">
                  <h1>Verification</h1>
                  <h1>{{header}}</h1>
                </div>
                <div class="content">
                  <p>Hello,</p>
                  <p>{{message}}</p>
                  <p>Thank you for registering with us. Please use the verification code below to verify your email address.</p>
                  <div class="verification-code">
                    <div class="verification-code-digit">{{code[0]}}</div>
                    <div class="verification-code-digit">{{code[1]}}</div>
                    <div class="verification-code-digit">{{code[2]}}</div>
                    <div class="verification-code-digit">{{code[3]}}</div>
                    <div class="verification-code-digit">{{code[4]}}</div>
                    <div class="verification-code-digit">{{code[5]}}</div>
                  </div>
                  <p>If you did not request this, please ignore this email.</p>
                  <p>Best regards, <br>Flvt </p>
                </div>
                <div class="footer">
                  <p>© 2024 Flvt All rights reserved.</p>
                </div>
              </div>
            </body>
          </html>
          """;

    public static string GenerateEmailVerificationMessage(string code) =>
        FromBaseVerificationCodeTemplate(
            "Verification",
            "Thank you for registering with us. Please use the verification code below to verify your email address.",
            code);

    public static string GenerateResetPasswordMessage(string code) =>
        FromBaseVerificationCodeTemplate(
            "New Password",
            "We received a request to reset your password. Please use the verification code below to reset your password.",
            code);

    public static string GenerateFilterLaunchNotificationMessage(string filterName, string filterId, int count) =>
        $$"""
           <!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
           <html dir="ltr" lang="en">
             <head>
               <meta charset="UTF-8">
               <title>Verification</title>
               <style type="text/css">
                 body {
                   font-family: Arial, sans-serif;
                   background-color: #f4f4f4;
                   margin: 0;
                   padding: 0;
                 }
           
                 .container {
                   width: 100%;
                   max-width: 600px;
                   margin: 0 auto;
                   background-color: #ffffff;
                   padding: 20px;
                   border-radius: 8px;
                   box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                 }
           
                 .header {
                   background-color: #44017a;
                   color: #ffffff;
                   padding: 10px 20px;
                   text-align: center;
                   border-radius: 8px 8px 0 0;
                 }
           
                 .content {
                   padding: 20px;
                   font-size: 16px;
                   line-height: 1.6;
                 }
           
                 .verification-code {
                   text-align: center;
                   display: block;
                   width: fit-content;
                   margin: 20px auto;
                   padding: 10px 20px;
                   background-color: #f4f4f4;
                   border: 1px solid #dddddd;
                   border-radius: 4px;
                   font-size: 24px;
                   font-weight: bold;
                   color: #44017a;
                 }
           
                 .verification-code-digit {
                   display: inline;
                   margin: 1rem;
                 }
           
                 .footer {
                   text-align: center;
                   padding: 10px;
                   font-size: 12px;
                   color: #aaaaaa;
                 }
           
                 ul li,
                 ol li {
                   margin-left: 0;
                 }
           
                 .es-p-default {
                   padding-top: 0px;
                   padding-right: 0px;
                   padding-bottom: 0px;
                   padding-left: 0px;
                 }
           
                 .es-p-all-default {
                   padding: 0px;
                 }
           
                 @media only screen and (max-width:600px) {
                   .h-auto {
                     height: auto !important
                   }
                 }
           
                 @media screen and (max-width:384px) {
                   .mail-message-content {
                     width: 414px !important
                   }
                 }
               </style>
             </head>
             <body data-new-gr-c-s-loaded="14.1204.0">
               <div class="container">
                 <div class="header">
                   <h1>Verification</h1>
                   <h1>New Advertisements</h1>
                 </div>
                 <div class="content">
                   <p>Hello,</p>
                   <p>We just found {{count}} new advertisements matching your filter. Click on the below link to browse them!</p>
                   <p>Thank you for registering with us. Please use the verification code below to verify your email address.</p>
                   <div class="verification-code">
                     <a href="https://flvt.eu/filters/{{filterId}}" target="_blank" style="text-decoration: none; color: #44017a;">
                       {{filterName}} on Flvt
                     </a>
                   </div>
                   <p>If you did not request this, please ignore this email.</p>
                   <p>Best regards, <br>Flvt </p>
                 </div>
                 <div class="footer">
                   <p>© 2024 Flvt All rights reserved.</p>
                 </div>
               </div>
             </body>
           </html>
           """;
}