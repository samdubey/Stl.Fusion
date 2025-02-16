namespace Stl.Tests.Serialization;

public class ExceptionInfoTest : TestBase
{
#pragma warning disable RCS1194
    public class WeirdException : Exception
#pragma warning restore RCS1194
    {
        public WeirdException() : base("") { }
    }

    public ExceptionInfoTest(ITestOutputHelper @out) : base(@out) { }

    [Fact]
    public void BasicTest()
    {
        var e = new Exception("1");
        var i = e.ToExceptionInfo();
        i.Message.Should().Be("1");
        i.ToException().Should().BeOfType<Exception>()
            .Which.Message.Should().Be("1");

        // ReSharper disable once NotResolvedInText
#pragma warning disable MA0015
        e = new ArgumentNullException("none", "2");
#pragma warning restore MA0015
        i = e.ToExceptionInfo();
        i.Message.Should().Be(e.Message);
        i.ToException().Should().BeOfType<ArgumentNullException>()
            .Which.Message.Should().Be(e.Message);

        // ReSharper disable once NotResolvedInText
        i = new WeirdException().ToExceptionInfo();
        i.Message.Should().Be("");
        ((RemoteException) i.ToException()!).ExceptionInfo.Should().Be(i);
    }

    [Fact]
    public void ResultExceptionTest()
    {
        var e = new Exception("1").ToResultException();
        var i = e.ToExceptionInfo();
        i.Message.Should().Be("1");
        var r = (ServiceException) i.ToException()!;
        r.Message.Should().Be("1");
        r.Unwrap().Should().BeOfType<Exception>().Which.Message.Should().Be("1");

        // ReSharper disable once NotResolvedInText
#pragma warning disable MA0015
        e = new ArgumentNullException("none", "2").ToResultException();
#pragma warning restore MA0015
        i = e.ToExceptionInfo();
        r = (ServiceException) i.ToException()!;
        r.Message.Should().Be(e.Message);
        r.Unwrap().Should().BeOfType<ArgumentNullException>()
            .Which.Message.Should().Be(e.Message);

        // ReSharper disable once NotResolvedInText
        e = new WeirdException().ToResultException();
        i = e.ToExceptionInfo();
        var re = (RemoteException) i.ToException()!;
        re.ExceptionInfo.Should().Be(i);
        Out.WriteLine(re.Message);
    }
}
