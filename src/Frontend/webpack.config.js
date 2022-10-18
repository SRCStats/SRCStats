const glob = require("glob")
const path = require("path");
const webpack = require("webpack")
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const CssMinimizerPlugin = require("css-minimizer-webpack-plugin");
const PurgeCSSPlugin = require("purgecss-webpack-plugin");
const TerserPlugin = require("terser-webpack-plugin");

module.exports = {
    entry: {
        site: './src/js/site.js',
        index: './src/js/index.js',
        user_search: './src/js/user_search.js',
        user_page: './src/js/user_page.js',
        webhooks: './src/js/webhooks.js'
    },
    output: {
        filename: '[name].js',
        path: path.resolve(__dirname, '..', 'wwwroot', 'js')
    },
    mode: 'production',
    devtool: 'source-map',
    module: {
        rules: [
            {
                test: /.s?css$/,
                use: [MiniCssExtractPlugin.loader, 'css-loader', 'sass-loader'],
            },
            {
                test: /\.(eot|woff(2)?|ttf|otf|svg)$/i,
                type: 'asset'
            }
        ]
    },
    optimization: {
        minimizer: [
            new CssMinimizerPlugin(),
            new TerserPlugin()
        ],
        minimize: true,
        usedExports: true
    },
    plugins: [
        new MiniCssExtractPlugin({
            filename: "../css/[name].css"
        }),
        new PurgeCSSPlugin({
          paths: glob.sync(`${path.join(__dirname, '..', 'Views')}/**/*.cshtml`,  { nodir: true })
        }),
        new webpack.ProvidePlugin({
            'window.Dropdown': ['bootstrap','Dropdown'],
        })
    ]
};
