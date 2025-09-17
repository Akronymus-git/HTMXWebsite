var path = require("path");
var webpack = require("webpack");

var cfg = {
    devtool: "source-map",
    entry: {incremental: "./distI/App.js", tools: "./distT/App.js", wplace: "./distW/App.js"},
    output: {
        path: path.join(__dirname, "public"),
        filename: "[name].js"
    },
    module: {
        rules: [
            {
                test: /\.js$/,
                exclude: /node_modules/,
                loader: "source-map-loader"
            }
        ]
    }
};

module.exports = cfg;