using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE.Service;

namespace DAL.Eso
{
   public class ServiceComponentFieldDal
    {
        private static readonly DatabaseContext Ctx = new DatabaseContext();

        public string AddServiceComponentField(ServiceComponentFieldsDto oServiceComponentFieldsDto, int nodeId,
            int systemUserId)
        {

            var id = new Common.Utils().GetPrimaryKey(nodeId, 35, "CF");

            oServiceComponentFieldsDto.v_ServiceComponentFieldsId = id;
            oServiceComponentFieldsDto.d_InsertDate = DateTime.Now;
            oServiceComponentFieldsDto.i_InsertUserId = systemUserId;
            oServiceComponentFieldsDto.i_IsDeleted = 0;
            
            Ctx.ServiceComponentFields.Add(oServiceComponentFieldsDto);
            Ctx.SaveChanges();

            return id;
        }

    }
}
