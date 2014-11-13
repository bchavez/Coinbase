using Coinbase.ObjectModel;
using FluentValidation;
using FluentValidation.Attributes;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace Coinbase.Tests
{
    [TestFixture]
    public class AbstractValidationTest<T>
    {
        private IValidatorFactory factory = new AttributedValidatorFactory();

        public IValidator<T> Validator { get; set; }

        [SetUp]
        public void BeforeEachTest()
        {
            this.Validator = factory.GetValidator<T>();
        }
    }

    public class ButtonValidatorTests : AbstractValidationTest<ButtonRequest>
    {
        [Test]
        public void subscription_should_have_a_repeat_value()
        {
            var request = new ButtonRequest
                {
                    Name = "test", Price = 79.99m, Currency = Currency.USD
                };

            request.Subscription = false;
            this.Validator.ShouldNotHaveValidationErrorFor(br => br.Repeat, request);

            request.Subscription = true;
            this.Validator.ShouldHaveValidationErrorFor(br => br.Repeat, request);

            request.Repeat = SubscriptionType.Monthly;
            this.Validator.ShouldNotHaveValidationErrorFor(br => br.Repeat, request);
        }
    }

}