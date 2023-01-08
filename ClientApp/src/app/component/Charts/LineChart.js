import React from 'react';
import ReactApexChart from 'react-apexcharts';

const LineChart = ({ title, data }) => {
    const [chartOptions, setChartOptions] = React.useState({});
    const [chartData, setChartData] = React.useState([]);

    React.useEffect(() => {

        const areaChartOption = {
            chart: {
                height: 10,
                type: 'area',
                toolbar: {
                    show: false
                }
            },
            dataLabels: {
                enabled: false
            },
            stroke: {
                curve: 'smooth'
            },
            xaxis: {
                categories: [...data.xAxisData]
            },
            yaxis: {
                min: 0,
                tickAmount: 10,
                labels: {
                    formatter: (value) => { return value.toFixed(0); }
                }
            }
        };
        setChartOptions(areaChartOption);

        const series = [{
            name: 'Temprature',
            data: [...data.tempatureSeriesData]
        }, {
            name: 'Humidity',
            data: [...data.humiditySeriesData]
        }];

        setChartData(series);
    }, [data]);

    return (
        <div className="card">
            <div className="card-body">
                <div className="clearfix mb-4">
                    <h4 className="card-title float-left">{title}</h4>
                </div>
                <ReactApexChart type='area' options={chartOptions} series={chartData} />
            </div>
        </div>
    );
};

export default LineChart;