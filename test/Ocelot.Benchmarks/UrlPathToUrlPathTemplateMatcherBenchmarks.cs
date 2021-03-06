using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using Ocelot.Library.Infrastructure.UrlPathMatcher;


namespace Ocelot.Benchmarks
{
    [Config(typeof(UrlPathToUrlPathTemplateMatcherBenchmarks))]
    public class UrlPathToUrlPathTemplateMatcherBenchmarks : ManualConfig
    {
        private UrlPathToUrlPathTemplateMatcher _urlPathMatcher;
        private string _downstreamUrlPath;
        private string _downstreamUrlPathTemplate;

        public UrlPathToUrlPathTemplateMatcherBenchmarks()
        {
             Add(StatisticColumn.AllStatistics);
        }
        
        [Setup]
        public void SetUp()
        {
            _urlPathMatcher = new UrlPathToUrlPathTemplateMatcher();
            _downstreamUrlPath = "api/product/products/1/variants/?soldout=false";
            _downstreamUrlPathTemplate = "api/product/products/{productId}/variants/";
        }

        [Benchmark]
        public void Benchmark1()
        {
            _urlPathMatcher.Match(_downstreamUrlPath, _downstreamUrlPathTemplate);
        }

        [Benchmark]
        public void Benchmark2()
        {
            _urlPathMatcher.Match(_downstreamUrlPath, _downstreamUrlPathTemplate);
        }
    }
}