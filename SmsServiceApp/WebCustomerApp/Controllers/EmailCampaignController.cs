﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BAL.Interfaces;
using BAL.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.ViewModels.EmailCampaignViewModels;
using Model.ViewModels.EmailRecipientViewModels;

namespace WebApp.Controllers
{
    [Authorize]
    public class EmailCampaignController : Controller
    {
        private IEmailCampaignManager emailCampaignManager;
        private IEmailRecipientManager emailRecipientManager;

        public EmailCampaignController(IEmailCampaignManager emailCampaignManager, IEmailRecipientManager emailRecipientManager)
        {
            this.emailCampaignManager = emailCampaignManager;
            this.emailRecipientManager = emailRecipientManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public List<EmailCampaignViewModel> Get(int page, int countOnPage, string searchValue)
        {
            if (searchValue == null)
                searchValue = "";
            if (!User.Identity.IsAuthenticated)
                return new List<EmailCampaignViewModel>();
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return emailCampaignManager.GetCampaigns(userId, page, countOnPage, searchValue);
        }

        public int GetCampaignsCount(string searchValue)
        {
            if (searchValue == null)
                searchValue = "";
            if (!User.Identity.IsAuthenticated)
                return 0;
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return emailCampaignManager.GetCampaignsCount(userId, searchValue);
        }

        public IActionResult Create()
        {
            return View();
        }

        public void CreateCampaign(EmailCampaignViewModel campaign, List<EmailRecipientViewModel> recepients)
        {
            if (!User.Identity.IsAuthenticated)
                return;
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            campaign.UserId = userId;
            emailCampaignManager.IncertWithRecepients(campaign, recepients);
        }

        public IActionResult Details(int campaignId)
        {
            var item = emailCampaignManager.GetById(campaignId);
            return View(item);
        }
    }
}
