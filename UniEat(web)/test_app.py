import pytest
from app import app

@pytest.fixture
def client():
    app.config['TESTING'] = True
    with app.test_client() as client:
        yield client

def test_home(client):
    response = client.get('/')
    assert response.status_code == 200
    assert b'Welcome to the UniEat App' in response.data

def test_login_invalid_credentials(client):
    response = client.post('/login', data={'username': 'admin', 'password': 'password'})
    assert response.status_code == 401
    assert b'Invalid username or password.' in response.data

def test_login_valid_credentials(client):
    response = client.post('/login', data={'username': 'validuser', 'password': 'validpassword'})
    assert response.status_code == 302  # Redirect
    assert b'Location' in response.headers
    assert b'/' in response.headers.get('Location')

def test_logout(client):
    response = client.get('/logout')
    assert response.status_code == 302  # Redirect
    assert b'Location' in response.headers
    assert b'/' in response.headers.get('Location')

def test_signup(client):
    response = client.post('/signup', data={'username': 'newuser', 'password': 'newpassword', 'confirm_password': 'newpassword', 'email': 'test@example.com'})
    assert response.status_code == 302  # Redirect
    assert b'Location' in response.headers
    assert b'/signup_success' in response.headers.get('Location')

def test_signup_existing_user(client):
    response = client.post('/signup', data={'username': 'existinguser', 'password': 'password', 'confirm_password': 'password', 'email': 'test@example.com'})
    assert response.status_code == 400
    assert b'Username already exists' in response.data

def test_vendors(client):
    response = client.get('/vendors')
    assert response.status_code == 200
    assert b'List of Vendors' in response.data

def test_detail(client):
    response = client.get('/detail/1')
    assert response.status_code == 404
    assert b'Vendor not found.' in response.data

def test_review(client):
    response = client.post('/review/1', data={'score': '5', 'comment': 'Great vendor'})
    assert response.status_code == 302  # Redirect
    assert b'Location' in response.headers
    assert b'/detail/1' in response.headers.get('Location')
