﻿using KInspector.Core.Constants;
using KInspector.Tests.Common.Helpers;
using KInspector.Reports.WebPartPerformanceAnalysis;
using KInspector.Reports.WebPartPerformanceAnalysis.Models;

using NUnit.Framework;

using System.Xml.Linq;

namespace KInspector.Tests.Common.Reports
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    [TestFixture(13)]
    public class WebPartPerformanceAnalysisTest : AbstractModuleTest<Report, Terms>
    {
        private readonly Report _mockReport;

        public WebPartPerformanceAnalysisTest(int majorVersion) : base(majorVersion)
        {
            _mockReport = new Report(_mockDatabaseService.Object, _mockModuleMetadataService.Object);
        }

        [Test]
        public void Should_ReturnGoodStatus_When_NoWebPartsHaveUnspecifiedColumns()
        {
            // Arrange
            ArrangeAllQueries();

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status == ResultsStatus.Good, $"Expected status when no web parts have unspecified columns is 'Good' not '{results.Status}'.");
        }

        [Test]
        public void Should_ReturnWarningStatus_When_ThereAreWebPartsWithUnspecifiedColumns()
        {
            // Arrange
            var affectedTemplates = new List<PageTemplate>();
            affectedTemplates.Add(new PageTemplate()
            {
                PageTemplateCodeName = "cms.blog",
                PageTemplateDisplayName = "Blog",
                PageTemplateID = 25807,
                PageTemplateWebParts = XDocument.Parse("<page><webpartzone id=\"zoneRight\"><webpart controlid=\"TagCloud\" guid=\"9be345f1-a4c1-4f57-a996-594e1aed0d4e\" type=\"TagCloud\"><property name=\"cachedependencies\">##DEFAULT##</property><property name=\"combinewithdefaultculture\">True</property><property name=\"container\">BlackBox</property><property name=\"containertitle\">Tags</property><property name=\"controlid\">TagCloud</property><property name=\"disablemacros\">False</property><property name=\"disableviewstate\">False</property><property name=\"documentlisturl\">/Blogs/My-blog-1</property><property name=\"hidecontrolforzerorows\">True</property><property name=\"hideonsubpages\">False</property><property name=\"instanceguid\">9be345f1-a4c1-4f57-a996-594e1aed0d4e</property><property name=\"maxtagsize\">20</property><property name=\"mintagsize\">10</property><property name=\"partialcachedependencies\">##DEFAULT##</property><property name=\"querystringname\">tagid</property><property name=\"selectonlypublished\">True</property><property name=\"taggroupname\">MyBlog</property><property name=\"timezonetype\">inherit</property><property name=\"usedocumentfilter\">False</property><property name=\"useupdatepanel\">False</property><property name=\"visible\">True</property><property name=\"webparttype\">TagCloud</property><property name=\"zerorowstext\">No data found</property></webpart><webpart controlid=\"rptSideColumn\" guid=\"390ccd7c-8216-4f8f-aacd-00380990d3b6\" type=\"repeater\"><property name=\"backnextlocation\">split</property><property name=\"cachedependencies\">##DEFAULT##</property><property name=\"checkpermissions\">False</property><property name=\"classnames\">cms.blog</property><property name=\"container\">BlackBox</property><property name=\"containertitle\">My favorite websites</property><property name=\"controlid\">rptSideColumn</property><property name=\"disablemacros\">False</property><property name=\"disableviewstate\">False</property><property name=\"enablepaging\">False</property><property name=\"filteroutduplicates\">False</property><property name=\"hidecontrolforzerorows\">False</property><property name=\"hideonsubpages\">False</property><property name=\"instanceguid\">390ccd7c-8216-4f8f-aacd-00380990d3b6</property><property name=\"maxrelativelevel\">-1</property><property name=\"orderby\">NodeLevel DESC</property><property name=\"pagenumbersseparator\">-</property><property name=\"pagerposition\">bottom</property><property name=\"pagesize\">10</property><property name=\"pagingmode\">querystring</property><property name=\"partialcachedependencies\">##DEFAULT##</property><property name=\"path\">/%</property><property name=\"relatednodeisontheleftside\">False</property><property name=\"relationshipname\">isrelatedto</property><property name=\"relationshipwithnodeguid\">00000000-0000-0000-0000-000000000000</property><property name=\"resultsposition\">top</property><property name=\"selectonlypublished\">True</property><property name=\"selecttopn\">1</property><property name=\"showeditdeletebuttons\">False</property><property name=\"showfirstlast\">True</property><property name=\"showfordocumenttypes\">cms.blog;cms.blogmonth;cms.blogpost</property><property name=\"shownewbutton\">False</property><property name=\"timezonetype\">inherit</property><property name=\"transformationname\">cms.blog.SideColumn</property><property name=\"useupdatepanel\">False</property><property name=\"visible\">True</property><property name=\"webparttype\">repeater</property><property name=\"zerorowstext\">No data found</property></webpart><webpart controlid=\"RecentPosts\" guid=\"964520b0-deab-47ff-9870-77b40235c7d1\" type=\"RecentPosts\"><property name=\"container\">BlackBox</property><property name=\"containertitle\">Recent posts</property><property name=\"controlid\">RecentPosts</property><property name=\"disablemacros\">False</property><property name=\"disableviewstate\">False</property><property name=\"hidecontrolforzerorows\">False</property><property name=\"hideonsubpages\">False</property><property name=\"instanceguid\">964520b0-deab-47ff-9870-77b40235c7d1</property><property name=\"partialcachedependencies\">##DEFAULT##</property><property name=\"partialcacheminutes\">20</property><property name=\"selecttopn\">5</property><property name=\"showfordocumenttypes\">cms.blog;cms.blogmonth;cms.blogpost</property><property name=\"timezonetype\">inherit</property><property name=\"transformationname\">cms.blogpost.RecentPosts</property><property name=\"useupdatepanel\">False</property><property name=\"visible\">True</property><property name=\"webparttype\">RecentPosts</property><property name=\"zerorowstext\">No recent posts</property></webpart><webpart controlid=\"PostArchive\" guid=\"4be02b0b-5205-4e2e-ad0e-9e148de15c9f\" type=\"PostArchive\"><property name=\"container\">BlackBox</property><property name=\"containertitle\">Post archive</property><property name=\"controlid\">PostArchive</property><property name=\"hidecontrolforzerorows\">False</property><property name=\"hideonsubpages\">False</property><property name=\"instanceguid\">4be02b0b-5205-4e2e-ad0e-9e148de15c9f</property><property name=\"showfordocumenttypes\">cms.blog;cms.blogmonth;cms.blogpost</property><property name=\"transformationname\">cms.blogpost.Archive</property><property name=\"visible\">True</property><property name=\"webparttype\">PostArchive</property><property name=\"zerorowstext\">No data in archive</property></webpart><webpart controlid=\"rptBlogDescription\" guid=\"d5b7eb38-6869-4e00-bb32-312808424ec1\" type=\"repeater\"><property name=\"backnextlocation\">split</property><property name=\"cachedependencies\">##DEFAULT##</property><property name=\"cacheminutes\">0</property><property name=\"checkpermissions\">False</property><property name=\"classnames\">cms.blog</property><property name=\"container\">BlackBox</property><property name=\"containertitle\">Title</property><property name=\"contentafter\">&lt;/div&gt;</property><property name=\"contentbefore\">&lt;div class=\"TextContent\"&gt;</property><property name=\"controlid\">rptBlogDescription</property><property name=\"disablemacros\">False</property><property name=\"disableviewstate\">False</property><property name=\"enablepaging\">False</property><property name=\"filteroutduplicates\">False</property><property name=\"hidecontrolforzerorows\">False</property><property name=\"hideonsubpages\">False</property><property name=\"instanceguid\">d5b7eb38-6869-4e00-bb32-312808424ec1</property><property name=\"maxrelativelevel\">-1</property><property name=\"orderby\">NodeLevel DESC</property><property name=\"pagenumbersseparator\">-</property><property name=\"pagerposition\">bottom</property><property name=\"pagesize\">10</property><property name=\"pagingmode\">querystring</property><property name=\"partialcachedependencies\">##DEFAULT##</property><property name=\"path\">/%</property><property name=\"relatednodeisontheleftside\">False</property><property name=\"relationshipname\">isrelatedto</property><property name=\"relationshipwithnodeguid\">00000000-0000-0000-0000-000000000000</property><property name=\"resultsposition\">top</property><property name=\"selectonlypublished\">True</property><property name=\"selecttopn\">1</property><property name=\"showeditdeletebuttons\">False</property><property name=\"showfirstlast\">True</property><property name=\"showfordocumenttypes\">cms.blog;cms.blogmonth;cms.blogpost</property><property name=\"shownewbutton\">False</property><property name=\"timezonetype\">inherit</property><property name=\"transformationname\">cms.blog.Description</property><property name=\"useupdatepanel\">False</property><property name=\"visible\">True</property><property name=\"webparttype\">repeater</property><property name=\"wherecondition\">'{%nodealiaspath%}' LIKE NodeAliasPath + '%'</property><property name=\"zerorowstext\">No data found</property></webpart></webpartzone><webpartzone id=\"zoneLeft\"><webpart controlid=\"rptAllPosts\" guid=\"8679aa66-d6f1-438a-8c03-c12034731651\" type=\"repeater\" v=\"1\"><property name=\"alternatingtransformationname\"></property><property name=\"backnextlocation\">split</property><property name=\"cachedependencies\">##DEFAULT##</property><property name=\"cacheitemname\"></property><property name=\"cacheminutes\">0</property><property name=\"cat_open_ajax\"></property><property name=\"cat_open_output_filter\"></property><property name=\"cat_open_performance\"></property><property name=\"cat_open_time zones\"></property><property name=\"cat_open_visibility\"></property><property name=\"categoryname\"></property><property name=\"checkpermissions\">False</property><property name=\"classnames\">cms.blogpost</property><property name=\"columns\"></property><property name=\"combinewithdefaultculture\"></property><property name=\"container\"></property><property name=\"containercssclass\"></property><property name=\"containercustomcontent\"></property><property name=\"containerhideonsubpages\">False</property><property name=\"containertitle\"></property><property name=\"contentafter\"></property><property name=\"contentbefore\"></property><property name=\"controlid\">rptAllPosts</property><property name=\"culturecode\"></property><property name=\"customtimezone\">Asuncion</property><property name=\"datasourcename\"></property><property name=\"disablemacros\">False</property><property name=\"disableviewstate\">False</property><property name=\"displaytoroles\"></property><property name=\"enableoutputfilter\">False</property><property name=\"enablepaging\">False</property><property name=\"filtername\"></property><property name=\"filteroutduplicates\">False</property><property name=\"hidecontrolforzerorows\">True</property><property name=\"hideonsubpages\">False</property><property name=\"instanceguid\">8679aa66-d6f1-438a-8c03-c12034731651</property><property name=\"itemseparator\"></property><property name=\"maxrelativelevel\">-1</property><property name=\"nestedcontrolsid\"></property><property name=\"newbuttontext\"></property><property name=\"orderby\">BlogPostDate DESC</property><property name=\"outputconverttablestodivs\">none</property><property name=\"outputfixattributes\">True</property><property name=\"outputfixhtml5\">True</property><property name=\"outputfixjavascript\">True</property><property name=\"outputfixlowercase\">True</property><property name=\"outputfixselfclose\">True</property><property name=\"outputfixtags\">True</property><property name=\"outputresolveurls\">True</property><property name=\"pagenumbersseparator\">-</property><property name=\"pagerhtmlafter\"></property><property name=\"pagerhtmlbefore\"></property><property name=\"pagerposition\">bottom</property><property name=\"pagesize\">10</property><property name=\"pagingmode\">postback</property><property name=\"partialcachedependencies\">##DEFAULT##</property><property name=\"partialcacheminutes\"></property><property name=\"path\"></property><property name=\"querystringkey\"></property><property name=\"relatednodeisontheleftside\">False</property><property name=\"relationshipname\">isrelatedto</property><property name=\"relationshipwithnodeguid\">00000000-0000-0000-0000-000000000000</property><property name=\"resultsposition\">top</property><property name=\"selecteditemtransformationname\">cms.blogpost.Default</property><property name=\"selectonlypublished\">True</property><property name=\"selecttopn\">5</property><property name=\"showeditdeletebuttons\">False</property><property name=\"showfirstlast\">True</property><property name=\"showfordocumenttypes\">cms.blog;cms.blogpost</property><property name=\"shownewbutton\">False</property><property name=\"sitename\"></property><property name=\"timezonetype\">custom</property><property name=\"transformationname\">cms.blog.PostPreview</property><property name=\"useupdatepanel\">False</property><property name=\"visible\">True</property><property name=\"webparttitle\"></property><property name=\"webparttype\">repeater</property><property name=\"wherecondition\">('{?ToInt(tagid, \"\")?}' = 0 AND N'{?tagname?}'=N'') OR (DocumentID IN (SELECT DocumentID FROM CMS_DocumentTag WHERE TagID = {?ToInt(tagid, \"\")?} )) OR (DocumentID IN (SELECT DocumentID FROM CMS_DocumentTag WHERE TagID IN (SELECT TagID FROM CMS_Tag WHERE TagName = N'{?tagname?}' AND TagGroupID = {?ToInt(groupid, \"\")?} )))</property><property name=\"zerorowstext\">No data found</property></webpart><webpart controlid=\"CommentView\" guid=\"39bbf7ba-5a9a-46bd-a72e-de50c724c2b7\" type=\"CommentView\"><property name=\"abusereportaccess\">0</property><property name=\"checkpermissions\">False</property><property name=\"commentseparator\">&lt;hr class=\"PostCommentSeparatorLine\" /&gt;</property><property name=\"contentafter\">&lt;/div&gt;</property><property name=\"contentbefore\">&lt;div class=\"comments TextContent\"&gt;</property><property name=\"controlid\">CommentView</property><property name=\"disablemacros\">False</property><property name=\"disableviewstate\">False</property><property name=\"displaytrackbacks\">True</property><property name=\"enableuserpictures\">False</property><property name=\"hideonsubpages\">False</property><property name=\"instanceguid\">39bbf7ba-5a9a-46bd-a72e-de50c724c2b7</property><property name=\"partialcachedependencies\">##DEFAULT##</property><property name=\"partialcacheminutes\">0</property><property name=\"showdeletebutton\">True</property><property name=\"showeditbutton\">True</property><property name=\"showfordocumenttypes\">cms.blogpost</property><property name=\"timezonetype\">inherit</property><property name=\"userpicturemaxheight\">50</property><property name=\"userpicturemaxwidth\">60</property><property name=\"useupdatepanel\">False</property><property name=\"visible\">True</property><property name=\"webparttype\">CommentView</property></webpart></webpartzone><webpartzone id=\"zoneTop\"><webpart controlid=\"Javascript\" guid=\"69bc22d5-36eb-4246-a6a6-4b261b85abb0\" type=\"javascript\" v=\"1\"><property name=\"cat_open_behavior\">True</property><property name=\"cat_open_performance\"></property><property name=\"controlid\">Javascript</property><property name=\"disablemacros\">False</property><property name=\"disableviewstate\">False</property><property name=\"displaytoroles\"></property><property name=\"generatescripttags\">True</property><property name=\"hideonsubpages\">False</property><property name=\"inlinescript\"></property><property name=\"inlinescriptpagelocation\">Header</property><property name=\"instanceguid\">69bc22d5-36eb-4246-a6a6-4b261b85abb0</property><property name=\"linkedfile\" ismacro=\"true\">jquery</property><property name=\"linkedfilepagelocation\">Header</property><property name=\"showfordocumenttypes\"></property><property name=\"visible\">True</property><property name=\"webparttitle\"></property><property name=\"webparttype\">javascript</property></webpart><webpart controlid=\"breadcrumbs\" guid=\"34e1d3c7-54c4-488a-a606-04a0175a2c48\" type=\"breadcrumbs\"><property name=\"applymenudesign\">True</property><property name=\"breadcrumbseparator\">&amp;gt;</property><property name=\"breadcrumbseparatorrtl\">&amp;lt;</property><property name=\"checkpermissions\">False</property><property name=\"classnames\">cms.blog;cms.blogmonth;cms.blogpost;CMS.MenuItem</property><property name=\"container\">Box.Gray</property><property name=\"controlid\">breadcrumbs</property><property name=\"disablemacros\">False</property><property name=\"disableviewstate\">False</property><property name=\"encodename\">True</property><property name=\"hidecontrolforzerorows\">True</property><property name=\"hideonsubpages\">False</property><property name=\"ignoreshowinnavigation\">False</property><property name=\"instanceguid\">34e1d3c7-54c4-488a-a606-04a0175a2c48</property><property name=\"renderlinktitle\">False</property><property name=\"selectnodesstartpath\">/</property><property name=\"selectonlypublished\">True</property><property name=\"showcurrentitem\">True</property><property name=\"showcurrentitemaslink\">False</property><property name=\"timezonetype\">inherit</property><property name=\"usertlbehaviour\">True</property><property name=\"useupdatepanel\">False</property><property name=\"visible\">True</property><property name=\"webparttype\">breadcrumbs</property><property name=\"zerorowstext\">No data found</property></webpart></webpartzone></page>")
            });

            ArrangeAllQueries(affectedTemplates);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status == ResultsStatus.Warning, $"Expected status when web parts have unspecified columns is 'Warning' not '{results.Status}'.");
        }

        private void ArrangeAllQueries(List<PageTemplate>? affectedTemplates = null)
        {
            affectedTemplates ??= new List<PageTemplate>();
            _mockDatabaseService.SetupExecuteSqlFromFile(Scripts.GetAffectedTemplates, affectedTemplates);

            var affectedTemplateIds = affectedTemplates.Select(x => x.PageTemplateID);
            var affectedDocuments = new List<Document>();
            _mockDatabaseService.SetupExecuteSqlFromFileWithListParameter(Scripts.GetDocumentsByPageTemplateIds, "IDs", affectedTemplateIds, affectedDocuments);
        }
    }
}