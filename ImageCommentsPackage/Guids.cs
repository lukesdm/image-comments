// Guids.cs
// MUST match guids.h
using System;

namespace LM.ImageComments.Package
{
    static class GuidList
    {
        public const string guidImageCommentsPackagePkgString = "B7EBBA5B-308D-4181-A115-7A8F92C1B85D";
        public const string guidImageCommentsPackageCmdSetString = "EC327262-6A34-4ECF-B7A1-E02C4AC70765";

        public static readonly Guid guidImageCommentsPackageCmdSet = new Guid(guidImageCommentsPackageCmdSetString);
    };
}