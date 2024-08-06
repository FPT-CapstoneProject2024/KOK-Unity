using System;

namespace KOK.ApiHandler.DTOModels
{
    public class UpPackage
    {
        public Guid? PackageId { get; set; }
        public string PackageName { get; set; }
        public string Description { get; set; }
        public decimal MoneyAmount { get; set; }
        public int? StarNumber { get; set; }
        public PackageStatus Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatorId { get; set; }
    }
}
