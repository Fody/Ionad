using ApprovalTests.Reporters;
#if(DEBUG)
[assembly: UseReporter(typeof(AllFailingTestsClipboardReporter), typeof(DiffReporter))]
#endif