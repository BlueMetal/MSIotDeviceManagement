//Style
import './css/site.css';

//Assets
//require.context('./assets/images', true, /^\.\//);

//Html
require.context('./app', true, /.html/);

//Javascript
import './app/app';
import './app/directives';
import './app/controllers';
import './app/services';