using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE.Service;

namespace DAL.Eso
{
    public class ServiceComponentFielValuesDal
    {

        private static readonly DatabaseContext Ctx = new DatabaseContext();

        public string AddServiceComponentFieldValue(ServiceComponentFieldValuesDto oServiceComponentFieldValuesDto, int nodeId,
            int systemUserId)
        {

            var id = new Common.Utils().GetPrimaryKey(nodeId, 36, "CV");

            oServiceComponentFieldValuesDto.v_ServiceComponentFieldsId = id;
            oServiceComponentFieldValuesDto.d_InsertDate = DateTime.Now;
            oServiceComponentFieldValuesDto.i_InsertUserId = systemUserId;
            oServiceComponentFieldValuesDto.i_IsDeleted = 0;

            Ctx.ServiceComponentFieldValues.Add(oServiceComponentFieldValuesDto);
            Ctx.SaveChanges();

            return id;
        }


    }
}
