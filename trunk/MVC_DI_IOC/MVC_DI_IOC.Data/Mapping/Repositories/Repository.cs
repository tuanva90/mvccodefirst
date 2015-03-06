using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_DI_IOC.Core.NorthWND.Data.Entities;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace MVC_DI_IOC.Data
{
    public class Repository<TEntity, TPrimaryKey> where TEntity : Entity<TPrimaryKey>
    {
        private readonly NorthWNDContext _context;
        private IDbSet<TEntity> _entities;
        string Errormsg = string.Empty;

        private IDbSet<TEntity> Entities
        {
            get
            {
                if (_entities == null)
                {
                    _entities = _context.Set<TEntity>();
                }
                return _entities;
            }
        }

        public Repository(NorthWNDContext context)
        {
            this._context = context;
        }
        public TEntity GetByID(object ID)
        {
            return this.Entities.Find(ID);
        }
        //Insert
        public void Insert(TEntity entity)
        {
            try
            {
                if(entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                this.Entities.Add(entity);
                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbex)
            {
                foreach(var validationErrors in dbex.EntityValidationErrors)
                {
                    foreach(var validationError in validationErrors.ValidationErrors)
                    {
                        Errormsg += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;
                    }
                }
                throw new Exception(Errormsg, dbex);
            }
        }
        //Update
        public void Update(TEntity entity)
        {
            try
            {
                if(entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbex)
            {
                foreach (var validationErrors in dbex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Errormsg += Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                throw new Exception(Errormsg, dbex);
            }
        }

        public virtual IQueryable<TEntity> Table
        {
            get { return this.Entities; }
        }
    }
}