using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Data;

namespace EdSofta.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        protected readonly ApplicationDbContext Context;

        public UnitOfWork()
        {
            Context = new ApplicationDbContext();
        }

        public bool SaveChanges()
        {
            var returnValue = true;
            using (var dbContextTransaction = Context.Database.BeginTransaction())
            {
                try
                {
                    Context.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch (Exception)
                {
                    //Log Exception Handling message                      
                    returnValue = false;
                    dbContextTransaction.Rollback();
                }
            }

            return returnValue;
        }

        public async Task<bool> SaveChangesAsync()
        {
            var returnValue = true;
            using (var dbContextTransaction = Context.Database.BeginTransaction())
            {
                try
                {
                    await Context.SaveChangesAsync();
                    dbContextTransaction.Commit();
                }
                catch (Exception)
                {
                    //Log Exception Handling message                      
                    returnValue = false;
                    dbContextTransaction.Rollback();
                }
            }

            return returnValue;
        }

        #region Public Properties  

        private QuestionRepository _questionRepository;
        public QuestionRepository QuestionRepository => _questionRepository ?? (_questionRepository = new QuestionRepository(Context));

        private UserRepository _userRepository;
        public UserRepository UserRepository => _userRepository ?? (_userRepository = new UserRepository(Context));

        private NotificationRepository _notificationRepository;
        public NotificationRepository NotificationRepository =>
            _notificationRepository ?? (_notificationRepository = new NotificationRepository(Context));

        private LRRepository _lrRepository;
        public LRRepository LrRepository => _lrRepository ?? (_lrRepository = new LRRepository(Context));

        private GameRepository _gameRepository;
        public GameRepository GameRepository => _gameRepository ?? (_gameRepository = new GameRepository(Context));


        #endregion


        #region IDisposable Support  
        private bool _disposedValue; // To detect redundant calls  

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;

            if (disposing)
            {
                //dispose managed state (managed objects). 
                Context.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override a finalizer below.  
            // set large fields to null.  

            _disposedValue = true;
        }

        // override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.  
        // ~UnitOfWork() {  
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.  
        //   Dispose(false);  
        // }  

        // This code added to correctly implement the disposable pattern.  
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.  
            Dispose(true);
            // uncomment the following line if the finalizer is overridden above.  
            // GC.SuppressFinalize(this);  
        }
        #endregion

    }
}
