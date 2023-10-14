from flask import Flask, render_template, request, redirect, session, abort
import os
import secrets
import logging
from model import setup_database, get_user, get_merchant, get_merchants, get_reviews, add_user, add_review, get_average_score


app = Flask(__name__)

# Generating a secret key for the Flask application
secret_key = secrets.token_hex(24)
app.secret_key = secret_key

# Configuring logging for the application
logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')


setup_database()

# Defining the route for the home page
@app.route('/')
def home():
    return render_template('index.html')

# Defining the route for the login page with support for GET and POST methods
@app.route('/login', methods=['GET', 'POST'])
def login():
    if request.method == 'POST':
        username = request.form.get('username')
        password = request.form.get('password')

        user = get_user(username, password)
        if user is None:
            logging.error('Invalid username or password')
            abort(401, 'Invalid username or password.')
        session['username'] = username
        return redirect('/')
    else:
        return render_template('login.html')

# Defining the route for the logout function
@app.route('/logout')
def logout():
    session.pop('username', None)
    return redirect('/')

# Defining the route for the signup page with support for GET and POST methods
@app.route('/signup', methods=['GET', 'POST'])
def signup():
    if request.method == 'POST':
        username = request.form.get('username')
        password = request.form.get('password')
        confirm_password = request.form.get('confirm_password')
        email = request.form.get('email')

        error_msg = add_user(username, password, confirm_password, email)
        if error_msg is not None:
            abort(400, error_msg)

        session['username'] = username
        return redirect('/signup_success')
    else:
        return render_template('signup.html')

# Defining the route for the signup success page
@app.route('/signup_success')
def signup_success():
    return render_template('signup_success.html')

# Defining the route for the merchants page
@app.route('/merchants')
def merchants():
    merchants = get_merchants()
    return render_template('list.html', merchants=merchants)

# Defining the route for the merchant details page
@app.route('/detail/<int:id>')
def detail(id):
    merchant = get_merchant(id)
    if merchant is None:
        abort(404, 'merchant not found.')
    reviews = get_reviews(id)
    avg_score = get_average_score(id)
    return render_template('detail.html', merchant=merchant, reviews=reviews, avg_score=avg_score)

# Defining the route for the review page with support for GET and POST methods
@app.route('/review/<int:merchant_id>', methods=['GET', 'POST'])
def review(merchant_id):
    if request.method == 'POST':
        score = request.form.get('score')
        comment = request.form.get('comment')

        add_review(merchant_id, score, comment)
        return redirect(f'/detail/{merchant_id}')
    else:
        return render_template('review.html', merchant_id=merchant_id)

# Defining error handlers for the application
@app.errorhandler(400)
def bad_request(error):
    return render_template('error.html', error_code=400, error_msg=error.description), 400

@app.errorhandler(401)
def unauthorized(error):
    return render_template('error.html', error_code=401, error_msg=error.description), 401

@app.errorhandler(404)
def not_found(error):
    return render_template('error.html', error_code=404, error_msg=error.description), 404


if __name__ == '__main__':
    app.run(debug=True)
