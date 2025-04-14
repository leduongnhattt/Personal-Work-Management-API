namespace PersonalWorkManagement.DTOs
{
    public class SocialLinkDTO
    {
        public string SocialLinkId { get; set; }
        public string Platform { get; set; }
        public string Url { get; set; }
    }

    public class CreateSocialLinkDTO
    {
        public string Platform { get; set; }
        public string Url { get; set; }
    }

    public class CreateMultipleSocialLinksDTO
    {
        public List<CreateSocialLinkDTO> SocialLinks { get; set; }
    }

    public class UpdateSocialLinkDTO
    {
        public string Url { get; set; }
    }
} 