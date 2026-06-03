namespace Interpreter.IntegrationTests.Features;

public sealed class IfStatementsTest
{
    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetIfBranchOutputs))]
    public void Can_execute_if_statements(string code, string expected)
    {
        FakeEnvironment environment = new();
        MinionInterpreter interpreter = new(environment);
        _ = interpreter.Execute(code);
        Assert.Equal(expected, environment.OutputBuffer);
        Assert.Equal(0, interpreter.ExitCode);
    }

    public static TheoryData<string, string> GetIfBranchOutputs()
    {
        return new TheoryData<string, string>
        {
            {
                """
                if (true) {
                  print(1);
                }
                """,
                "1"
            },
            {
                """
                if (false) {
                  print(1);
                }
                print(2);
                """,
                "2"
            },
            {
                """
                if (false) {
                }
                print(1);
                """,
                "1"
            },
            {
                """
                if (1 < 2) {
                  print("yes");
                } else {
                  print("no");
                }
                """,
                "yes"
            },
            {
                """
                if (2 < 1) {
                  print("yes");
                } else {
                  print("no");
                }
                """,
                "no"
            },
            {
                """
                if (true) {
                  print(1);
                  print(2);
                }
                """,
                "12"
            },
            {
                """
                let x: Int = 1;
                if (x == 1) {
                  print(10);
                } else {
                  if (x == 2) {
                    print(20);
                  } else {
                    print(30);
                  }
                }
                """,
                "10"
            },
            {
                """
                let x: Int = 2;
                if (x == 1) {
                  print(10);
                } else {
                  if (x == 2) {
                    print(20);
                  } else {
                    print(30);
                  }
                }
                """,
                "20"
            },
            {
                """
                let x: Int = 3;
                if (x == 1) {
                  print(10);
                } else {
                  if (x == 2) {
                    print(20);
                  } else {
                    print(30);
                  }
                }
                """,
                "30"
            },
            {
                """
                let x: Int = 0;
                if (x == 1) {
                  print(1);
                } else {
                  if (x == 2) {
                    print(2);
                  } else {
                    if (x == 3) {
                      print(3);
                    } else {
                      print(4);
                    }
                  }
                }
                """,
                "4"
            },
            {
                """
                if (true) {
                  if (false) {
                    print(1);
                  } else {
                    print(2);
                  }
                }
                """,
                "2"
            },
            {
                """
                var n: Int = 0;
                if (true) {
                  n = 5;
                }
                print(n);
                """,
                "5"
            },
            {
                """
                if (true) {
                  var local: Int = 42;
                  print(local);
                }
                """,
                "42"
            },
            {
                """
                var n: Int = 1;
                {
                  if (true) {
                    let n: Int = 9;
                    print(n);
                  }
                }
                print(n);
                """,
                "91"
            },
            {
                """
                if (false && ((1 / 0) == 1)) {
                  print(1);
                } else {
                  print(2);
                }
                """,
                "2"
            },
            {
                """
                if (true) {
                  print(1);
                } else {
                  print(2);
                }
                print(3);
                """,
                "13"
            },
        };
    }

    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetInvalidIfPrograms))]
    public void Reject_invalid_if_conditions(string code, Type exceptionType)
    {
        FakeEnvironment environment = new();
        MinionInterpreter interpreter = new(environment);
        Exception? ex = null;
        try
        {
            _ = interpreter.Execute(code);
        }
        catch (Exception e)
        {
            ex = e;
        }

        Assert.NotNull(ex);
        Assert.Equal(exceptionType, ex.GetType());
    }

    public static TheoryData<string, Type> GetInvalidIfPrograms()
    {
        return new TheoryData<string, Type>
        {
            {
                """
                if (1) {
                  print(1);
                }
                """,
                typeof(InvalidOperationException)
            },
            {
                """
                if (true && ((1 / 0) == 1)) {
                  print(1);
                }
                """,
                typeof(DivideByZeroException)
            },
        };
    }
}
