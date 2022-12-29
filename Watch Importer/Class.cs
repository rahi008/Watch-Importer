using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watch_Importer
{
    public class watch
    {
        public long ID { get; set; }
        public string Title { get; set; }
        public string Brand { get; set; }
        public string Description { get; set; }
        public string Series { get; set; }
        public string StockNumber { get; set; }
        public string ModelNumber { get; set; }
        public string Gender { get; set; }
        public string BandMaterial { get; set; }
        public string FaceColor { get; set; }
        public string HandIndicators { get; set; }
        public string BandSize { get; set; }
        public string Crystal { get; set; }
        public string MovementType { get; set; }
        public string CaseWidth { get; set; }
        public string CaseHeight { get; set; }
        public string ClaspType { get; set; }
        public string Bezel { get; set; }
        public string CaseDiameter { get; set; }
        public string CaseThickness { get; set; }
        public string CaseMaterial { get; set; }
        public string CaseBack { get; set; }
        public string Bracelet_Strap { get; set; }
        public string WaterResistant { get; set; }
        public string BatteryLife { get; set; }
        public string Warranty { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string Image3 { get; set; }
        public int ListPrice { get; set; }
        public int Price { get; set; }
        public int Scat { get; set; }
        public string PT { get; set; }
        public string BraceletLength { get; set; }
        public string Features { get; set; }
        public string crown { get; set; }
        public int Quantity { get; set; }
        public string SupplierStock { get; set; }
        public string UPC { get; set; }
    }
    public class watches
    {
        public string Brand { get; set; }
        public string Series { get; set; }
        public string SKU_Code { get; set; }
        public string Description { get; set; }
        public string MSRP { get; set; }
        public string UPC { get; set; }

    }
}
