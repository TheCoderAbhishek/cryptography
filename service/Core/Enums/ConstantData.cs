namespace service.Core.Enums
{
    /// <summary>
    /// Provides utility methods for generating constant data.
    /// </summary>
    public static class ConstantData
    {
        /// <summary>
        /// Generates a unique transaction ID based on current time.
        /// </summary>
        /// <returns>A string representing the transaction ID in the format "yyyyMMddHHmmssfff".</returns>
        public static string Txn()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssfff");
        }

        /// <summary>
        /// Professional and Modern email body.
        /// </summary>
        /// <param name="otp">Dynamically generated OTP.</param>
        /// <returns>Return a HTML string for email body template</returns>
        public static string GetProfessionalOtpHtmlBody(string otp)
        {
            return $@"
                    <html>
                    <head>
                        <style>
                            body {{
                                font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif;
                                background-color: #f8f9fa;
                                margin: 0;
                                padding: 0;
                            }}
                            .container {{
                                max-width: 600px;
                                margin: 0 auto;
                                background-color: #ffffff;
                                padding: 20px;
                                border: 1px solid #e9ecef;
                                border-radius: 5px;
                                box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                            }}
                            .header {{
                                background-color: #28a745;
                                color: white;
                                padding: 15px;
                                text-align: center;
                                font-size: 20px;
                                border-radius: 5px 5px 0 0;
                            }}
                            .body {{
                                padding: 20px;
                                text-align: center;
                            }}
                            .otp {{
                                font-size: 32px;
                                font-weight: bold;
                                color: #28a745;
                                margin: 20px 0;
                            }}
                            .footer {{
                                margin-top: 20px;
                                font-size: 14px;
                                color: #6c757d;
                                text-align: center;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header'>Secure OTP Code</div>
                            <div class='body'>
                                <p>Hello,</p>
                                <p>Your OTP code is:</p>
                                <p class='otp'>{otp}</p>
                                <p>This code is valid for the next 15 minutes. Please use it promptly to complete your request.</p>
                            </div>
                            <div class='footer'>
                                <p>Best Regards,</p>
                                <p>The Ayerhs Team</p>
                            </div>
                        </div>
                    </body>
                    </html>";
        }

        /// <summary>
        /// Gets or sets the path to the keystore file.
        /// </summary>
        public static string? KeyStorePath { get; set; } = null;

        /// <summary>
        /// The file name for the DSA parameters.
        /// </summary>
        public const string DsaParameterFileName = @"dsa_param.pem";

        /// <summary>
        /// The file name for the DSA private key.
        /// </summary>
        public const string DsaKeyFileName = @"dsa_key.pem";
    }
}
