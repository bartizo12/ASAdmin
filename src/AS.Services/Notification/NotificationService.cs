using AS.Domain.Entities;
using AS.Infrastructure.Data;
using AS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace AS.Services
{
    /// <summary>
    /// Interface for notification service that deals with creating/displaying notifications
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly IDbContext _dbContext;

        public NotificationService(IDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        /// <summary>
        /// Gets unseen notification record count to display it as an indicator
        /// </summary>
        /// <param name="userId">Id of the user</param>
        /// <returns>Count of unseen notifications of the user</returns>
        public int GetUnseenCount(int userId)
        {
            var query = this._dbContext.Set<Notification>().AsNoTracking() as IQueryable<Notification>;
            query = query.Where(notification => !notification.IsSeen);
            query = query.Where(notification => notification.UserId == userId);

            return query.Count();
        }

        /// <summary>
        /// Inserts notification
        /// </summary>
        /// <param name="notification">Notification to be inserted</param>
        /// <returns>Inserted notification</returns>
        public Notification Insert(Notification notification)
        {
            notification = this._dbContext.Set<Notification>().Add(notification);
            this._dbContext.SaveChanges();

            return notification;
        }

        /// <summary>
        /// Inserts notification
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="message"></param>
        /// <param name="url"></param>
        /// <returns>Inserted notification</returns>
        public Notification Insert(int userId, string message, string url)
        {
            Notification notification = new Notification(userId, message, url);
            return this.Insert(notification);
        }

        /// <summary>
        /// Gets list of user notification up to <paramref name="count"/>
        /// </summary>
        /// <param name="userId">Id of the user</param>
        /// <param name="count">Max count</param>
        /// <returns>List of notification</returns>
        public IList<Notification> GetNotifications(int userId, int count)
        {
            var query = this._dbContext.Set<Notification>().AsNoTracking() as IQueryable<Notification>;
            query = query.Where(notification => notification.UserId == userId);
            query = query.OrderByDescending(notification => notification.CreatedOn);
            query = query.Take(count);

            return query.ToList();
        }

        /// <summary>
        /// Updates notifications status as "Seen"
        /// </summary>
        /// <param name="notificationIds">Id list of the notifications to be updated </param>
        public void UpdateAsSeen(IEnumerable<int> notificationIds)
        {
            var query = this._dbContext.Set<Notification>() as IQueryable<Notification>;
            query = query.Where(n => notificationIds.Contains(n.Id));

            foreach (Notification notification in query)
            {
                notification.IsSeen = true;
                notification.SeenOn = DateTime.UtcNow;
                _dbContext.Entry(notification).State = EntityState.Modified;
            }
            this._dbContext.SaveChanges();
        }
    }
}