using FluentAssertions;
using GatheringTools.LogoutControl;
using NUnit.Framework;

namespace GatheringTools.UnitTest
{
    public class VisibilityService_Test
    {
        [TestCase(true, true, true)]
        [TestCase(true, false, true)]
        [TestCase(false, true, true)]
        [TestCase(false, false, true)]
        public void Return_isVisible_when_1_2_3_true(bool isInGame,
                                                     bool mapIsOpen,
                                                     bool expectedIsVisible)
        {
            var show               = true;
            var showOnMap          = true;
            var showOnSelectAndCut = true;

            var result = VisibilityService.ShouldBeVisible(
                show,
                showOnMap,
                showOnSelectAndCut,
                isInGame,
                mapIsOpen == false);

            result.Should().Be(expectedIsVisible);
        }

        [TestCase(true, true, true)]
        [TestCase(true, false, true)]
        [TestCase(false, true, false)]
        [TestCase(false, false, false)]
        public void Return_isVisible_when_only_1_2_true(bool isInGame,
                                                        bool mapIsOpen,
                                                        bool expectedIsVisible)
        {
            var show               = true;
            var showOnMap          = true;
            var showOnSelectAndCut = false;

            var result = VisibilityService.ShouldBeVisible(
                show,
                showOnMap,
                showOnSelectAndCut,
                isInGame,
                mapIsOpen == false);

            result.Should().Be(expectedIsVisible);
        }

        [TestCase(true, true, false)]
        [TestCase(true, false, true)]
        [TestCase(false, true, false)]
        [TestCase(false, false, true)]
        public void Return_isVisible_when_only_1_3_true(bool isInGame,
                                                        bool mapIsOpen,
                                                        bool expectedIsVisible)
        {
            var show               = true;
            var showOnMap          = false;
            var showOnSelectAndCut = true;

            var result = VisibilityService.ShouldBeVisible(
                show,
                showOnMap,
                showOnSelectAndCut,
                isInGame,
                mapIsOpen == false);

            result.Should().Be(expectedIsVisible);
        }

        [TestCase(true, true, false)]
        [TestCase(true, false, true)]
        [TestCase(false, true, false)]
        [TestCase(false, false, false)]
        public void Return_isVisible_when_only_1_true(bool isInGame,
                                                      bool mapIsOpen,
                                                      bool expectedIsVisible)
        {
            var show               = true;
            var showOnMap          = false;
            var showOnSelectAndCut = false;

            var result = VisibilityService.ShouldBeVisible(
                show,
                showOnMap,
                showOnSelectAndCut,
                isInGame,
                mapIsOpen == false);

            result.Should().Be(expectedIsVisible);
        }
        
        [TestCase(true, true, false)]
        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        [TestCase(false, false, false)]
        public void Return_isVisible_when_only_2_3_true(bool isInGame,
                                                        bool mapIsOpen,
                                                        bool expectedIsVisible)
        {
            var show               = false;
            var showOnMap          = true;
            var showOnSelectAndCut = true;

            var result = VisibilityService.ShouldBeVisible(
                show,
                showOnMap,
                showOnSelectAndCut,
                isInGame,
                mapIsOpen == false);

            result.Should().Be(expectedIsVisible);
        }

        [TestCase(true, true, false)]
        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        [TestCase(false, false, false)]
        public void Return_isVisible_when_only_2_true(bool isInGame,
                                                        bool mapIsOpen,
                                                        bool expectedIsVisible)
        {
            var show               = false;
            var showOnMap          = true;
            var showOnSelectAndCut = false;

            var result = VisibilityService.ShouldBeVisible(
                show,
                showOnMap,
                showOnSelectAndCut,
                isInGame,
                mapIsOpen == false);

            result.Should().Be(expectedIsVisible);
        }

        [TestCase(true, true, false)]
        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        [TestCase(false, false, false)]
        public void Return_isVisible_when_only_3_true(bool isInGame,
                                                      bool mapIsOpen,
                                                      bool expectedIsVisible)
        {
            var show               = false;
            var showOnMap          = false;
            var showOnSelectAndCut = true;

            var result = VisibilityService.ShouldBeVisible(
                show,
                showOnMap,
                showOnSelectAndCut,
                isInGame,
                mapIsOpen == false);

            result.Should().Be(expectedIsVisible);
        }

        [TestCase(true, true, false)]
        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        [TestCase(false, false, false)]
        public void Return_isVisible_when_none_true(bool isInGame,
                                                    bool mapIsOpen,
                                                    bool expectedIsVisible)
        {
            var show               = false;
            var showOnMap          = false;
            var showOnSelectAndCut = false;

            var result = VisibilityService.ShouldBeVisible(
                show,
                showOnMap,
                showOnSelectAndCut,
                isInGame,
                mapIsOpen == false);

            result.Should().Be(expectedIsVisible);
        }
    }
}