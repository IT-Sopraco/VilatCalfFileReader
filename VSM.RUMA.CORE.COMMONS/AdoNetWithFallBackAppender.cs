using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Appender;
using log4net.Core;
using System.Data;
using log4net.Layout;
using log4net.Util;

namespace VSM.Logger
{
    public class AdoNetWithFallBackAppender : log4net.Appender.AdoNetAppender, IAppenderAttachable
    {

        ~AdoNetWithFallBackAppender()
        {
            this.Flush();
        }

        override protected void SendBuffer(LoggingEvent[] events)
        {
            if (base.ReconnectOnError && (base.Connection == null || base.Connection.State != ConnectionState.Open))
            {
                base.SendBuffer(events);
                return;
            }

            // Check that the connection exists and is open
            if (base.Connection != null && base.Connection.State == ConnectionState.Open)
            {
                if (base.UseTransactions)
                {
                    // Create transaction
                    // NJC - Do this on 2 lines because it can confuse the debugger
                    IDbTransaction dbTran = null;
                    try
                    {
                        dbTran = base.Connection.BeginTransaction();

                        SendBuffer(dbTran, events);

                        // commit transaction
                        dbTran.Commit();
                    }
                    catch (Exception ex)
                    {
                        if (iAppenderAttachedImpl != null)
                        {
                            iAppenderAttachedImpl.AppendLoopOnAppenders(events);
                        }

                        // rollback the transaction
                        if (dbTran != null)
                        {
                            try
                            {
                                dbTran.Rollback();
                            }
                            catch (Exception)
                            {
                                // Ignore exception
                            }
                        }

                        // Can't insert into the database. That's a bad thing
                        ErrorHandler.Error("Exception while writing to database", ex);
                    }
                }
                else
                {
                    // Send without transaction
                    SendBuffer(null, events);
                }
            }

        }



        #region Implementation of IAppenderAttachable

        private AppenderAttachedImpl iAppenderAttachedImpl;
        /// <summary>
        /// Adds an <see cref="IAppender" /> to the list of appenders of this
        /// instance.
        /// </summary>
        /// <param name="newAppender">The <see cref="IAppender" /> to add to this appender.</param>
        /// <remarks>
        /// <para>
        /// If the specified <see cref="IAppender" /> is already in the list of
        /// appenders, then it won't be added again.
        /// </para>
        /// </remarks>
        virtual public void AddAppender(IAppender newAppender)
        {
            if (newAppender == null)
            {
                throw new ArgumentNullException("newAppender");
            }
            lock (this)
            {
                if (iAppenderAttachedImpl == null)
                {
                    iAppenderAttachedImpl = new log4net.Util.AppenderAttachedImpl();
                }
                iAppenderAttachedImpl.AddAppender(newAppender);
            }
        }

        /// <summary>
        /// Gets the appenders contained in this appender as an 
        /// <see cref="System.Collections.ICollection"/>.
        /// </summary>
        /// <remarks>
        /// If no appenders can be found, then an <see cref="EmptyCollection"/> 
        /// is returned.
        /// </remarks>
        /// <returns>
        /// A collection of the appenders in this appender.
        /// </returns>
        virtual public AppenderCollection Appenders
        {
            get
            {
                lock (this)
                {
                    if (iAppenderAttachedImpl == null)
                    {
                        return AppenderCollection.EmptyCollection;
                    }
                    else
                    {
                        return iAppenderAttachedImpl.Appenders;
                    }
                }
            }
        }

        /// <summary>
        /// Looks for the appender with the specified name.
        /// </summary>
        /// <param name="name">The name of the appender to lookup.</param>
        /// <returns>
        /// The appender with the specified name, or <c>null</c>.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Get the named appender attached to this buffering appender.
        /// </para>
        /// </remarks>
        virtual public IAppender GetAppender(string name)
        {
            lock (this)
            {
                if (iAppenderAttachedImpl == null || name == null)
                {
                    return null;
                }

                return iAppenderAttachedImpl.GetAppender(name);
            }
        }

        /// <summary>
        /// Removes all previously added appenders from this appender.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is useful when re-reading configuration information.
        /// </para>
        /// </remarks>
        virtual public void RemoveAllAppenders()
        {
            lock (this)
            {
                if (iAppenderAttachedImpl != null)
                {
                    iAppenderAttachedImpl.RemoveAllAppenders();
                    iAppenderAttachedImpl = null;
                }
            }
        }

        /// <summary>
        /// Removes the specified appender from the list of appenders.
        /// </summary>
        /// <param name="appender">The appender to remove.</param>
        /// <returns>The appender removed from the list</returns>
        /// <remarks>
        /// The appender removed is not closed.
        /// If you are discarding the appender you must call
        /// <see cref="IAppender.Close"/> on the appender removed.
        /// </remarks>
        virtual public IAppender RemoveAppender(IAppender appender)
        {
            lock (this)
            {
                if (appender != null && iAppenderAttachedImpl != null)
                {
                    return iAppenderAttachedImpl.RemoveAppender(appender);
                }
            }
            return null;
        }

        /// <summary>
        /// Removes the appender with the specified name from the list of appenders.
        /// </summary>
        /// <param name="name">The name of the appender to remove.</param>
        /// <returns>The appender removed from the list</returns>
        /// <remarks>
        /// The appender removed is not closed.
        /// If you are discarding the appender you must call
        /// <see cref="IAppender.Close"/> on the appender removed.
        /// </remarks>
        virtual public IAppender RemoveAppender(string name)
        {
            lock (this)
            {
                if (name != null && iAppenderAttachedImpl != null)
                {
                    return iAppenderAttachedImpl.RemoveAppender(name);
                }
            }
            return null;
        }

        #endregion Implementation of IAppenderAttachable

    }

}
