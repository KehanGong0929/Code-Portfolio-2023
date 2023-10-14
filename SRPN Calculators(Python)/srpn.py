# This is your SRPN file. Make your changes here.
import operator
import random

# Introduce the embeded operator function.
ops = {
  '+': operator.add,
  '-': operator.sub,
  '*': operator.mul,
  '/': operator.truediv,
  '^': operator.pow,
  '%': operator.mod,
}

# The module of deciding if a string is digit number or not.
def is_number(test_str):
    if test_str == None:
        return 0
        # For the return values from process_command to prevent 'NoneType'.
    else:
        try:
            int(test_str)
            return True
        except ValueError:
            pass

#  Set the satuation for the result.
def get_saturation(Num):
    if int(Num) > 2**31 - 1:
        return 2**31 - 1
    elif int(Num) < -2**31:
        return -2**31
    else:
        return Num

# For sorting out the different function from resolved input characters.
# Controled by main function and send result to process_control.
def process_command(command):
    if command == '=':
        if len(stack) >= 1:
            print(stack[len(stack) - 1])
        else:
            print('Stack Empty')
    elif command == 'r':
    # Generate random possitive number.
        return random.randint(0, 2147483647)

    elif command == 'd': # List the elements and print -2**31 if empty.
        if len(stack) >= 1:
            i = 0
            while i < len(stack):
                print(stack[i])
                i += 1
        else:
            print(-2**31)

    elif command == '' or command == ' ': # Resolve the empty command.
        return None

    elif is_number(command) or command in "+-*/%^":
    # Deliver the result to calculate function
        return command
    else:
        print('Unrecognised operator or operand "%s". ' % command)
        # For the charactors have no functions.

# Controled by process_control function and get the numbers from stack.
def calculate(Opt):
    result = 0
    Sat_result = 0
    l = len(stack)
    if len(stack) < 2:
        print('Stack underflow.')
        # The sign of refusing calculation for the limited numbers in stack.
    elif stack[-1] == '0' and Opt in '/%':
        print('Divide by 0.') # x/0 is not acceptable.
    elif int(stack[-1]) < 0 and Opt == '^':
        print('Negative power.') # Negative power is not acceptable.
    else:
        n1 = int(stack.pop(l - 1))
        n2 = int(stack.pop(l - 2))
        result = int(ops[Opt](n2, n1))
        Sat_result = get_saturation(result)
        # Get the satuated result and push it to the stack.
        stack.append(str(Sat_result))

# Remove the comments between two '#'.
def remove_pound(target_str):
    ret = ''
    skip = 0
    for i in target_str:
        if i == '#':
            skip += 1
        elif i == '#' and skip > 0:
            skip -= 1
        elif skip == 0:
            ret += i
    return ret

# Push to stack if a number.
# Send to calculate function if an operator.
def process_control(Obj):
    Sat_Obj = 0
    if is_number(Obj):
        if len(stack) < 23: # The stack can only contain maximum 23 elements.
            Sat_Obj = get_saturation(Obj)
            stack.append(str(Sat_Obj))
        else:
            print('Stack overflow.')
    elif is_number(Obj) == 0:
    # Resolve return value of None from process_command during number judgement.
        pass
    else:
        calculate(Obj)


def main(Input):
    # Deciding an Input is a lateral equation or not.
    if is_number(Input) or len(Input) <= 1:
        process_control(process_command(Input))
    else:
        Input_rm = remove_pound(Input)
        Input_list = Input_rm.split()
        # Resolve the equation, split it by ' ', and send them to a new list.
        for i in Input_list:
            process_control(process_command(i))


#This is the entry point for the program.
#It is suggested that you do not edit the below,
#to ensure your code runs with the marking script
if __name__ == "__main__":
    stack = []
    while True:
        try:
            cmd = input()
        except EOFError:
            exit()
        else:
            main(cmd)
