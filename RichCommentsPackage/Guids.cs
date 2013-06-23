// Guids.cs
// MUST match guids.h
using System;

namespace LM.RichComments.Package
{
    static class GuidList
    {
        public const string guidRichCommentsPackagePkgString = "2980fb0d-ba9d-4373-9178-d27a98f0c9b1";
        public const string guidRichCommentsPackageCmdSetString = "75607cbc-1a36-4075-8eb7-3ca58ae87f84";

        public static readonly Guid guidRichCommentsPackageCmdSet = new Guid(guidRichCommentsPackageCmdSetString);
    };
}