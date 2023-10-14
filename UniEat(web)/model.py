import duckdb
import re
import logging

# Function to connect to the database
def connect_db():
    try:
        conn = duckdb.connect('uniEat.duckdb')
        return conn
    except Exception as e:
        logging.error(f"Failed to connect to the database: {str(e)}")
        raise

def create_merchants_table(conn):
    try:
        cursor = conn.cursor()

        cursor.execute("""
        CREATE TABLE IF NOT EXISTS merchants (
            id INTEGER PRIMARY KEY,
            name VARCHAR,
            description TEXT,
            location VARCHAR
        )
        """)

        cursor.close()
    except Exception as e:
        logging.error(f"Failed to create the 'merchants' table: {str(e)}")
        raise

def create_users_table(conn):
    try:
        cursor = conn.cursor()

        cursor.execute("""
        CREATE TABLE IF NOT EXISTS users (
            user_id INTEGER PRIMARY KEY,
            username VARCHAR,
            password VARCHAR,
            email VARCHAR
        )
        """)

        cursor.close()
    except Exception as e:
        logging.error(f"Failed to create the 'users' table: {str(e)}")
        raise


def create_reviews_table(conn):
    try:
        cursor = conn.cursor()

        cursor.execute("""
        CREATE TABLE IF NOT EXISTS reviews (
            review_id INTEGER PRIMARY KEY,
            merchant_id INTEGER,
            score INTEGER,
            comment TEXT
        )
        """)

        cursor.close()
    except Exception as e:
        logging.error(f"Failed to create the 'reviews' table: {str(e)}")
        raise

def insert_sample_merchants(conn):
    try:
        cursor = conn.cursor()

        cursor.execute("SELECT id FROM merchants WHERE id = 1")
        if cursor.fetchone() is None:
            cursor.execute("""
            INSERT INTO merchants (id, name, description, location) 
            VALUES 
                (1, 'merchant 1', 'This is merchant 1', 'Location 1'),
                (2, 'merchant 2', 'This is merchant 2', 'Location 2'),
                (3, 'merchant 3', 'This is merchant 3', 'Location 3')
            """)

        conn.commit()
        cursor.close()
    except Exception as e:
        logging.error(f"Failed to insert sample merchants: {str(e)}")
        raise

# This function sets up the database. It creates a connection, initializes the tables, 
def setup_database():
    conn = connect_db()  
    create_merchants_table(conn)  
    create_users_table(conn) 
    create_reviews_table(conn) 
    insert_sample_merchants(conn)  
    conn.close()  

# This function is responsible for adding a new user to the users table in the database.
def add_user(username, password, confirm_password, email):
    try:
        # Validate the format of the email
        if not re.match(r'^[\w\.-]+@[\w\.-]+\.\w+$', email):
            logging.error('Invalid email format')
            return 'Error: Invalid email format.'

        # Check if the password and confirm password fields match
        if password != confirm_password:
            logging.error('Password and Confirm Password do not match')
            return 'Error: Password and Confirm Password do not match.'

        # Validate the length of the password
        if len(password) < 8 or len(confirm_password) < 8:
            logging.error('Password must be at least 8 characters long')
            return 'Error: Passwords must be at least 8 characters long.'

   
        conn = connect_db()
        cursor = conn.cursor()

        # Check if the username already exists in the database
        cursor.execute("SELECT * FROM users WHERE username = ?", (username,))
        if cursor.fetchone() is not None:
            logging.error('Username already exists')
            return 'Error: Username already exists.'

        # Check if the email already exists in the database
        cursor.execute("SELECT * FROM users WHERE email = ?", (email,))
        if cursor.fetchone() is not None:
            logging.error('Email already in use')
            return 'Error: Email already in use.'

        # Get the maximum user_id in the database, so we can add 1 to it for the new user
        cursor.execute("SELECT MAX(user_id) FROM users")
        max_id = cursor.fetchone()[0]

        if max_id is None:  
            max_id = 0

        new_id = max_id + 1

        # Insert the new user into the users table
        cursor.execute("""
            INSERT INTO users (user_id, username, password, email)
            VALUES (?, ?, ?, ?)
        """, (new_id, username, password, email))

        conn.commit()
        cursor.close()  
        conn.close()  
    except Exception as e:
        logging.error(f"Failed to add user: {str(e)}")
        raise

# This function retrieves a user from the users table by username and password.
def get_user(username, password):
    try:
        conn = connect_db() 
        cursor = conn.cursor()


        cursor.execute("SELECT * FROM users WHERE username = ? AND password = ?", (username, password))
        user = cursor.fetchone()  

        cursor.close() 
        conn.close()  

        return user  
    except Exception as e:
        logging.error(f"Failed to get user: {str(e)}")
        raise

# This function retrieves a merchant from the merchants table by merchant_id.
def get_merchant(merchant_id):
    try:
        conn = connect_db()  
        cursor = conn.cursor()


        cursor.execute("SELECT * FROM merchants WHERE id = ?", (merchant_id,))
        merchant = cursor.fetchone() 

        cursor.close() 
        conn.close()  

        return merchant  
    except Exception as e:
        logging.error(f"Failed to get merchant: {str(e)}")
        raise

# This function retrieves all the merchants from the merchants table.
def get_merchants():
    try:
        conn = connect_db()  
        cursor = conn.cursor()


        cursor.execute("SELECT * FROM merchants")
        merchants = cursor.fetchall()  

        cursor.close()  
        conn.close()  

        return merchants  
    except Exception as e:
        logging.error(f"Failed to get merchants: {str(e)}")
        raise

# This function retrieves all the reviews for a particular merchant by merchant_id.
def get_reviews(merchant_id):
    try:
        conn = connect_db()  
        cursor = conn.cursor()

  
        cursor.execute("SELECT score, comment FROM reviews WHERE merchant_id = ?", (merchant_id,))
        reviews = cursor.fetchall()  

        cursor.close()  
        conn.close()  

        return reviews  
    except Exception as e:
        logging.error(f"Failed to get reviews: {str(e)}")
        raise

# This function calculates the average score for a particular merchant by merchant_id.
def get_average_score(merchant_id):
    try:
        conn = connect_db()  
        cursor = conn.cursor()

        # Execute the query to calculate the average score
        cursor.execute("SELECT AVG(score) FROM reviews WHERE merchant_id = ?", (merchant_id,))
        avg_score = cursor.fetchone()[0]
        
        # If avg_score is None (i.e., there are no reviews), we set it to 0. Otherwise, we round it to 1 decimal place
        avg_score = round(avg_score, 1) if avg_score is not None else 0

        cursor.close()  
        conn.close() 

        return avg_score  
    except Exception as e:
        logging.error(f"Failed to get average score: {str(e)}")
        raise

# This function adds a review to the reviews table in the database.
def add_review(merchant_id, score, comment):
    try:
        conn = connect_db()  
        cursor = conn.cursor()

      
        cursor.execute("SELECT MAX(review_id) FROM reviews")
        max_id = cursor.fetchone()[0]

        if max_id is None:  
            max_id = 0

        new_id = max_id + 1


        cursor.execute("""
            INSERT INTO reviews (review_id, merchant_id, score, comment)
            VALUES (?, ?, ?, ?)
        """, (new_id, merchant_id, score, comment))

        conn.commit()  
        cursor.close()  
        conn.close()  
    except Exception as e:
        logging.error(f"Failed to add review: {str(e)}")
        raise
