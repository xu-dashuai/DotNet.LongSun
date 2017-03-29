using Lunson.Domain;
using Lunson.Domain.Entities;
using Pharos.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunson.DAL.Repositories
{
    public class AnnexRepository
    {
        private EFDbContext context = ContextFactory.GetCurrentContext();

        public Annex CreateAnnex(Annex annex,string currentUserID)
        {
            annex.CreatedBy = currentUserID;
            annex.CreatedOn = DateTime.Now;
            annex.ID = DataHelper.GetSystemID();
            annex.IsDeleted = YesOrNo.No;
            context.Annexes.Add(annex);
            context.SaveChanges();
            return annex;
        }
        public Annex GetAnnex(string id)
        {
            return context.Annexes.SingleOrDefault(a => a.IsDeleted == YesOrNo.No && a.ID.Equals(id));
        }
        public Annex GetAnnex(string id,AnnexType type)
        {
            return context.Annexes.SingleOrDefault(a => a.IsDeleted == YesOrNo.No && a.ID.Equals(id)&&a.Type==type);
        }
        public IQueryable<Annex> GetAnnexes(AnnexType type)
        {
            return context.Annexes.Where(a => a.Type == type && a.IsDeleted == YesOrNo.No).OrderByDescending(a => a.CreatedBy);
        }

        public Annex RemoveAnnex(string id, string currentUserID, AnnexType type)
        {
            var annex = GetAnnex(id,type);
            if (annex != null)
            {
                annex.IsDeleted = YesOrNo.Yes;
                annex.ModifiedOn = DateTime.Now;
                annex.ModifiedBy = currentUserID;
                context.SaveChanges();
            }
            return annex;
        }


        public void EditAnnexName(string id, string name, string currentUserID)
        {
            var annex = GetAnnex(id,AnnexType.Swf);
            if (annex != null && name.IsNullOrTrimEmpty() == false)
            {
                annex.OldName = name.Trim();
                annex.ModifiedBy = currentUserID;
                annex.ModifiedOn = DateTime.Now;
                context.SaveChanges();
            }
        }
    }
}
