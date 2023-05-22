const path = require("path");
const { CleanWebpackPlugin } = require("clean-webpack-plugin");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");

module.exports = {
    entry: {
        "index": "./src/index.ts"
    },
  output: {
    path: path.resolve(__dirname, "wwwroot/webpack"),
    filename: "[name].js",
    publicPath: "/",
  },
  resolve: {
    extensions: [".js", ".ts"],
  },
  module: {
    rules: [
      {
        test: /\.ts$/,
        use: "ts-loader",
      },
          {
              test: /\.s?css$/,
              use: [
                  // Save the CSS as a separate file to allow caching                            
                  MiniCssExtractPlugin.loader,
                  {
                      // Translate CSS into CommonJS modules
                      loader: 'css-loader',
                  },
                  {
                      // Run postcss actions
                      loader: 'postcss-loader',
                      options: {
                          postcssOptions: {
                              plugins: [
                                  function () {
                                      return [require('autoprefixer')];
                                  }
                              ],
                          },
                      },
                  },
                  {
                      loader: 'sass-loader',
                      options: {
                          sassOptions: {
                              outputStyle: "compressed",
                          }
                      }
                  }
              ],
          },
          {
              test: /\.woff($|\?)|\.woff2($|\?)|\.ttf($|\?)|\.eot($|\?)|\.svg($|\?)/i,
              type: 'asset/resource',
              generator: {
                  //filename: 'fonts/[name]-[hash][ext][query]'
                  filename: './fonts/[name][ext][query]'
              }
          }

    ],
  },
  plugins: [
    new CleanWebpackPlugin(),
    new MiniCssExtractPlugin({
      filename: "css/[name].css",
    }),
  ],
};
