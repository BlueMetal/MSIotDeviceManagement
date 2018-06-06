const path = require('path');
const webpack = require('webpack');
const CommonsChunkPlugin = require("webpack/lib/optimize/CommonsChunkPlugin"); //for vendor libraries to avoid reloading static code
const ExtractTextPlugin = require('extract-text-webpack-plugin'); //For production builds it's recommended to extract the CSS from bundle

module.exports = (env) => {
    const isDevBuild = !(env && env.prod);

    const config = {
        entry: {
            main: './ClientApp/main.ts',
            vendor: './ClientApp/vendor.ts'
        },
        output: {
            path: __dirname + '/wwwroot/dist',
            filename: '[name].js',
            chunkFilename: '[name].js',
            publicPath: '/dist/'
        },
        resolve: {
            extensions: ['.js', '.ts']
        },
        module: {
            rules: [
                {
                    test: /\.ts$/,
                    enforce: 'pre',
                    loader: 'ts-loader',
                    options: {}
                },
                {
                    test: /\.css$/,
                    loaders: !isDevBuild ?
                        ExtractTextPlugin.extract({
                            fallback: 'style-loader',
                            use: ['css-loader?minimize']
                        })
                        : ['style-loader', 'css-loader']
                },
                /*{
                    test: /\.scss$/,
                    loaders: !isDevBuild ?
                        ExtractTextPlugin.extract({
                            fallback: 'style-loader',
                            use: ['css-loader?minimize', 'sass-loader']
                        })
                        : ['style-loader', 'css-loader', 'sass-loader']
                },*/
                {
                    test: /\.woff(2)?(\?v=[0-9]\.[0-9]\.[0-9])?$/,
                    loader: "url-loader?limit=10000&mimetype=application/font-woff"
                },
                {
                    test: /\.(ttf|eot)(\?v=[0-9]\.[0-9]\.[0-9])?$/,
                    loader: "file-loader"
                },
                {
                    test: /\.html$/,
                    loader: !isDevBuild ? "raw-loader!html-minify-loader" : "raw-loader",
                    exclude: /node_modules/
                }
                //Not handled by webpack, directly wwwroot
                ,
                {
                    test: /\.(svg)$/,
                    loader: "file-loader",
                    options: {
                        context: 'ClientApp/dist/',
                        name: "[path][name].[ext]"
                    }
                }
            ]
        },
        plugins: [
            new CommonsChunkPlugin({
                names: ['vendor'],
                minChunks: Infinity
            }),

            new ExtractTextPlugin({
                filename: '[name].css',
                disable: isDevBuild
            })
        ],
        devtool: 'source-map'
    };

    //TODO FIX UGLIFY FOR PROD
    //Minimize JS if prod build
    /*if (!isDevBuild)
        config.plugins.push(new webpack.optimize.UglifyJsPlugin({
            minimize: !isDevBuild,
            compress: {
                warnings: false
            }
        }));*/

    return config;
};