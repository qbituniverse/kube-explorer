using System;

namespace qu.nuget.azure.Dto
{
    public class UserProfileDto
    {
        public string odatametadata { get; set; }
        public Value[] value { get; set; }

        public class Value
        {
            public string odatatype { get; set; }
            public string objectType { get; set; }
            public string objectId { get; set; }
            public object deletionTimestamp { get; set; }
            public bool accountEnabled { get; set; }
            public object[] assignedLicenses { get; set; }
            public object[] assignedPlans { get; set; }
            public object city { get; set; }
            public object companyName { get; set; }
            public object country { get; set; }
            public object creationType { get; set; }
            public string department { get; set; }
            public object dirSyncEnabled { get; set; }
            public string displayName { get; set; }
            public object employeeId { get; set; }
            public object facsimileTelephoneNumber { get; set; }
            public string givenName { get; set; }
            public object immutableId { get; set; }
            public object isCompromised { get; set; }
            public string jobTitle { get; set; }
            public object lastDirSyncTime { get; set; }
            public object mail { get; set; }
            public string mailNickname { get; set; }
            public object mobile { get; set; }
            public object onPremisesDistinguishedName { get; set; }
            public object onPremisesSecurityIdentifier { get; set; }
            public string[] otherMails { get; set; }
            public object passwordPolicies { get; set; }
            public object passwordProfile { get; set; }
            public object physicalDeliveryOfficeName { get; set; }
            public object postalCode { get; set; }
            public object preferredLanguage { get; set; }
            public object[] provisionedPlans { get; set; }
            public object[] provisioningErrors { get; set; }
            public object[] proxyAddresses { get; set; }
            public DateTime refreshTokensValidFromDateTime { get; set; }
            public object showInAddressList { get; set; }
            public object[] signInNames { get; set; }
            public object sipProxyAddress { get; set; }
            public object state { get; set; }
            public object streetAddress { get; set; }
            public string surname { get; set; }
            public object telephoneNumber { get; set; }
            public object usageLocation { get; set; }
            public object[] userIdentities { get; set; }
            public string userPrincipalName { get; set; }
            public string userType { get; set; }
        }
    }
}