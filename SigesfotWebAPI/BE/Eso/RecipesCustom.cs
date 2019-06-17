using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE.Eso
{
    public class RecipesCustom
    {
        public class BoradTransferProducts
        {
            public int? NodeId { get; set; }
            public int? InsertUserId { get; set; }
            public int? IsLocallyProcessed { get; set; }
            public List<BoardPrintRecipes> List { get; set; }
        }
        public class BoardPrintRecipes
        {
            public string WarehouseId { get; set; }
            public string ServiceId { get; set; }
            public string SupplierId { get; set; }
            public string PersonId { get; set; }
            public string ReferenceDocument { get; set; }
            public int? NodeId { get; set; }
            public int? InsertUserId { get; set; }
            public int? MovementTypeId { get; set; }
            public int? MotiveTypeId { get; set; }
            public DateTime? Date { get; set; }
            public List<Recomendations> ListRecomendations { get; set; }
            public List<Restrictions> ListRestrictions { get; set; }
            public List<Recipes> ListProducts { get; set; }
        }

        public class Recipes
        {
            public string MovementId { get; set; }           
            public string ProductId { get; set; }
            public string DiseaseName { get; set; }
            public string ProductName { get; set; }
            public string Posologia { get; set; }
            public string Duration { get; set; }
            public float? Quantity { get; set; }
            public float? StockActual { get; set; }
        }

        public class Recomendations
        {
            public string RecomendationName { get; set; }
        }

        public class Restrictions
        {
            public string RestrictionName { get; set; }
        }
    }
}
