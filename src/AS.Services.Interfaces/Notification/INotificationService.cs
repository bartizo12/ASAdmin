using AS.Domain.Entities;
using System.Collections.Generic;

namespace AS.Services.Interfaces
{
    /// <summary>
    /// Interface for notification service that deals with creating/displaying notifications
    /// </summary>
    public interface INotificationService : IService
    {
        /// <summary>
        /// Gets unseen notification record count to display it as an indicator
        /// </summary>
        /// <param name="userId">Id of the user</param>
        /// <returns>Count of unseen notifications of the user</returns>
        int GetUnseenCount(int userId);

        /// <summary>
        /// Inserts notification
        /// </summary>
        /// <param name="notification">Notification to be inserted</param>
        /// <returns>Inserted notification</returns>
        Notification Insert(Notification notification);

        /// <summary>
        /// Inserts notification
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="message"></param>
        /// <param name="url"></param>
        /// <returns>Inserted notification</returns>
        Notification Insert(int userId, string message, string url);

        /// <summary>
        /// Gets list of user notification up to <paramref name="count"/>
        /// </summary>
        /// <param name="userId">Id of the user</param>
        /// <param name="count">Max count</param>
        /// <returns>List of notification</returns>
        IList<Notification> GetNotifications(int userId, int count);

        /// <summary>
        /// Updates notifications status as "Seen"
        /// </summary>
        /// <param name="notificationIds">Id list of the notifications to be updated </param>
        void UpdateAsSeen(IEnumerable<int> notificationIds);
    }
}